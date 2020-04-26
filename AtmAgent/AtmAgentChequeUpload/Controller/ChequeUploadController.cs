using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AtmAgent.Cheques;
using Microsoft.Rest;
using CafeLib.Core.Eventing;
using CafeLib.Core.Extensions;
using CafeLib.Core.Runnable;
using CafeLib.Core.Support;
using CafeLib.Data;
using AtmAgentChequeUpload.Builder;
using AtmAgentChequeUpload.Config;
using AtmAgentChequeUpload.Files;
using AtmAgentChequeUpload.Logging;
using AtmAgentChequeUpload.Maintenance;
using AtmAgentChequeUpload.Messaging;
using AtmAgentChequeUpload.Messaging.Base;
using AtmAgentChequeUpload.Models;
using AtmAgentChequeUpload.Uploading;
using CafeLib.Data.Extensions;
// ReSharper disable UnusedMember.Global

namespace AtmAgentChequeUpload.Controller
{
    public class ChequeUploadController : IServiceController
    {
        private readonly IChequeFileManager _fileManager;
        private readonly IStorage _storage;
        private readonly ILogger _logger;
        private readonly UploadChequeQueue _uploadQueue;
        private readonly ChequeFileMonitor _fileMonitor;
        private readonly ChequeBuilder _chequeBuilder;

        private readonly MaintenanceTask _maintainer;
        private readonly RecurrentTask _chequeLoader;

        public ChequeUploadController(
            IAppConfig config,
            IEventService eventService,
            IChequeFileManager fileManager,
            IChequeParser parser,
            IDatabase database,
            ILogger logger)
        {
            _fileManager = fileManager;
            _storage = database.GetStorage();
            _logger = logger;
            _fileMonitor = new ChequeFileMonitor(eventService, fileManager, logger);
            _chequeBuilder = new ChequeBuilder(config, eventService, fileManager, parser, logger);
            _uploadQueue = new UploadChequeQueue(new UploadProcess(config, eventService, fileManager, logger));

            _maintainer = new MaintenanceTask(config, fileManager, async () => await MaintenanceHandler(config, eventService, logger));
            _chequeLoader = new RecurrentTask(ReloadCheques, TimeSpan.FromMinutes(config.ArchiveRetryMinutes), DateTime.Now.Add(TimeSpan.FromMinutes(config.ArchiveRetryMinutes)));

            eventService.Subscribe<ChequeFileMessage>(AddChequeFile);
            eventService.Subscribe<ChequeReadyMessage>(async x => await ChequeHandler(x));
            eventService.Subscribe<ChequeIncompleteMessage>(async x => await ChequeHandler(x));
            eventService.Subscribe<ChequeExpiryMessage>(async x => await ChequeTerminated(config, x));
            eventService.Subscribe<ChequeCancelledMessage>(async x => await ChequeTerminated(config, x));
            eventService.Subscribe<UploadSuccessMessage>(async x => await SuccessfulUpload(x));
            eventService.Subscribe<UploadFailMessage>(async x => await FailedUpload(x));
            eventService.Subscribe<UploadRetryFailMessage>(async x => await FailedUpload(x));
        }

        public bool IsRunning { get; private set; }

        public async Task Start()
        {
            if (IsRunning) return;
            try
            {
                _logger.Info($"Starting {nameof(ChequeUploadController)}...");
                await _chequeBuilder.Start();
                await _fileMonitor.Start();
                await _uploadQueue.Start();
                await _maintainer.Start();
                await _chequeLoader.Start();
                IsRunning = true;
                _logger.Info($"{nameof(ChequeUploadController)} started");
            }
            catch (Exception ex)
            {
                _logger.Warn($"{nameof(ChequeUploadController)} start failure.", ex);
                IsRunning = false;
            }
        }

        public async Task Stop()
        {
            if (!IsRunning) return;
            try
            {
                _logger.Debug($"Stopping {nameof(ChequeUploadController)}...");
                await _fileMonitor.Stop();
                await _chequeBuilder.Stop();
                await _uploadQueue.Stop();
                await _maintainer.Stop();
                await _chequeLoader.Stop();
            }
            catch (Exception ex)
            {
                _logger.Warn($"Exception occurred while stopping {nameof(ChequeUploadController)}", ex);
            }
            finally
            {
                _logger.Warn($"{nameof(ChequeUploadController)} stopped.");
                IsRunning = false;
            }
        }

        public void Dispose()
        {
            Task.Run(async () =>
            {
                await Stop();
                _chequeBuilder.Dispose();
                _chequeLoader.Dispose();
                _maintainer.Dispose();
            });
        }

        #region Message Handlers

        private async Task ChequeHandler(ChequeStatusMessage message)
        {
            switch (message)
            {
                case ChequeReadyMessage ready:
                    {
                        var cheque = await _storage.FindOne<Cheque>(x => x.ChequeId == ready.Cheque.ChequeId);
                        if (cheque?.Status != ChequeStatus.Uploaded)
                        {
                            _logger.Debug($"Queueing cheque {ready.Cheque.ChequeId}.");
                            _uploadQueue.Produce(await _storage.Save(ready.Cheque));
                        }
                        else
                        {
                            _logger.Debug($"Removing cheque files of previously uploaded cheque {ready.Cheque.ChequeId}.");
                            await DeleteChequeFiles(ready.Cheque);
                        }

                        return;
                    }

                case ChequeIncompleteMessage inc:
                    {
                        if (inc.Cheque.IsEmpty())
                        {
                            _logger.Warn($"No cheque files found. Purge cheque {inc.Cheque.ChequeId}.");
                            await _storage.Remove(inc.Cheque);
                        }
                        else
                        {
                            _logger.Warn($"Detected incomplete cheque {inc.Cheque.ChequeId}.");
                            await _storage.Save(inc.Cheque);
                        }
                        return;
                    }
            }
        }

        private async Task ChequeTerminated(IAppConfig config, ChequeTerminatedMessage message)
        {
            _logger.Info($"{message.Status} cheque {message.Cheque.ChequeId}.");
            switch (config.DatabaseMaintenanceMode)
            {
                case MaintenanceMode.Default:
                case MaintenanceMode.Remove:
                    await _storage.Remove(message.Cheque);
                    await DeleteChequeFiles(message.Cheque);
                    DeleteArchiveFiles(message.Cheque);
                    break;

                case MaintenanceMode.Retain:
                    _logger.Info($"Archiving cheque - {message.Cheque.ChequeId}.");
                    await ArchiveChequeFiles(message.Cheque);
                    await _storage.Save(message.Cheque);
                    break;
            }
        }

        private void AddChequeFile(ChequeFileMessage message)
        {
            const string fileFound = "file found:";
            const string fileCreated = "file created:";
            const string fileDeleted = "file deleted:";

            var isMetadata = _fileManager.IsMetadataFile(message.FileName);
            var isImage = _fileManager.IsImageFile(message.FileName);

            var prefix = isMetadata
                ? "Metadata"
                : isImage
                    ? "Image"
                    : "Non cheque";

            switch (message.Change)
            {
                case ChequeFileChange.Found:
                    _logger.Info($"{prefix} {fileFound} {message.FileName}.");
                    if (isMetadata || isImage)
                    {
                        _chequeBuilder.AddFile(message.FileName);
                    }

                    break;

                case ChequeFileChange.Created:
                    _logger.Info($"{prefix} {fileCreated} {message.FileName}.");
                    if (isMetadata || isImage)
                    {
                        _chequeBuilder.AddFile(message.FileName);
                    }

                    break;

                case ChequeFileChange.Deleted:
                    _logger.Info($"{prefix} {fileDeleted} {message.FileName}.");
                    break;
            }
        }

        private async Task SuccessfulUpload(UploadMessage message)
        {
            _logger.Info($"Successfully uploaded files for cheque: {message.Cheque.ChequeId}.");

            // Delete the cheque files.
            await DeleteChequeFiles(message.Cheque);

            // Save successful cheque.
            await _storage.Save(message.Cheque);
        }

        private async Task FailedUpload(UploadMessage message)
        {
            _logger.Warn($"Failed uploading files for cheque '{message.Cheque.ChequeId}'.");
            switch (message)
            {
                case UploadRetryFailMessage r:
                    _logger.Warn($"Retry limit of {r.Attempts} reached for cheque {r.Cheque.ChequeId}.");
                    await _storage.Save(r.Cheque);
                    break;

                case UploadFailMessage f:
                    for (var exception = f.Exception; exception != null; exception = exception.InnerException)
                    {
                        var errorMessage = $"Reason: {exception.Message}.";
                        _logger.Warn($"Exception Type: {exception.GetType().Name}.");

                        switch (exception)
                        {
                            //case HttpException http:
                            //    _logger.Warn($"Web event code: {http.WebEventCode}");
                            //    _logger.Warn($"Html error message: {http.GetHtmlErrorMessage()}");
                            //    _logger.Warn(errorMessage);
                            //    break;

                            case HttpRequestException _:
                                _logger.Warn(errorMessage);
                                break;

                            case HttpOperationException op:
                                _logger.Warn($"Uri: {op.Request.RequestUri}");
                                _logger.Warn($"Request Method: {op.Request.Method.Method}");
                                _logger.Warn($"Request Content Length: {op.Request.Content.Length}");
                                _logger.Warn($"Response Status code: {op.Response.StatusCode.Humanize()}");
                                _logger.Warn($"Response Description: {op.Response.ReasonPhrase}");
                                _logger.Warn($"Response Content: {op.Response.Content}");
                                _logger.Warn(errorMessage);
                                break;

                            case WebException web:
                                _logger.Warn($"Uri: {web.Response?.ResponseUri}");
                                _logger.Warn($"Exception status: {web.Status.Humanize()}");

                                if (web.Response is HttpWebResponse response)
                                {
                                    _logger.Warn($"Status code: {response.StatusCode.Humanize()}");
                                    _logger.Warn($"Description: {response.StatusDescription}");
                                }

                                _logger.Warn(errorMessage);
                                break;

                            case { } ex:
                                _logger.Warn(errorMessage);
                                _logger.Warn(string.Empty, ex);
                                break;
                        }
                    }

                    break;
            }
        }

        #endregion

        #region Callbacks

        /// <summary>
        /// Maintenance handler.
        /// </summary>
        /// <returns></returns>
        private async Task MaintenanceHandler(IAppConfig config, IEventService eventService, ILogger logger)
        {
            // Log running of maintenance handler.
            logger.Info($"Running '{config.DatabaseMaintenanceMode}' Maintenance Mode on {DateTime.Now}.");

            // Removed uploaded cheques from the database within the maintenance interval.
            var cheques = (await _storage.Find<Cheque>(x => x.Status == ChequeStatus.Uploaded))?
                .Where(x => DateTime.Now.Subtract(x.CreationDate) >= config.DatabaseMaintenanceInterval);
            await _storage.Remove(cheques);

            // Cancel all the incomplete cheques from the database within the maintenance interval.
            cheques = (await _storage.Find<Cheque>(x => x.Status == ChequeStatus.Incomplete))?
                .Where(x => DateTime.Now.Subtract(x.CreationDate) >= config.DatabaseMaintenanceInterval);
            cheques?.ForEach(x => eventService.Publish(new ChequeCancelledMessage(x)));

            // Expire all failed cheques in the database within the maintenance interval.
            cheques = (await _storage.Find<Cheque>(x => x.Status == ChequeStatus.Failed))?
                .Where(x => DateTime.Now.Subtract(x.CreationDate) >= config.DatabaseMaintenanceInterval);
            cheques?.ForEach(x => eventService.Publish(new ChequeExpiryMessage(x)));

            if (config.DatabaseMaintenanceMode != MaintenanceMode.Retain)
            {
                // Remove all terminated cheques from the database & archive within the maintenance interval.
                var chequeList = (await _storage.Find<Cheque>(x => x.Status == ChequeStatus.Cancelled))?
                    .Where(x => DateTime.Now.Subtract(x.LastUpdateDate) >= config.DatabaseMaintenanceInterval).ToList() ?? new List<Cheque>();

                chequeList.AddRange((await _storage.Find<Cheque>(x => x.Status == ChequeStatus.Expired))?
                    .Where(x => DateTime.Now.Subtract(x.LastUpdateDate) >= config.DatabaseMaintenanceInterval) ?? new List<Cheque>());

                await _storage.Remove(chequeList);
                chequeList.ForEach(DeleteArchiveFiles);
            }
        }

        /// <summary>
        /// Reloaded failed cheques.
        /// </summary>
        /// <returns></returns>
        private async Task ReloadCheques()
        {
            // Reload failed upload cheques.
            (await _storage.Find<Cheque>(x => x.Status == ChequeStatus.Failed))?
                .ForEach(x => _chequeBuilder.AddCheque(x));

            // Reload incomplete cheques.
            (await _storage.Find<Cheque>(x => x.Status == ChequeStatus.Incomplete))?
                .ForEach(x => _chequeBuilder.AddCheque(x));
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Archive cheque files.
        /// </summary>
        /// <param name="cheque">cheque</param>
        private async Task ArchiveChequeFiles(Cheque cheque)
        {
            try
            {
                await Retry.Run(x =>
                {
                    _fileManager.ArchiveChequeFile(cheque.MetadataFile);
                    _fileManager.ArchiveChequeFile(cheque.FrontImageFile);
                    _fileManager.ArchiveChequeFile(cheque.RearImageFile);
                });
            }
            catch (Exception ex)
            {
                _logger.Warn($"Error occurred during archiving of cheque files for cheque {cheque.ChequeId}.", ex);
            }
        }

        /// <summary>
        /// Delete cheque files.
        /// </summary>
        /// <param name="cheque">cheque</param>
        private async Task DeleteChequeFiles(Cheque cheque)
        {
            try
            {
                await Retry.Run(x =>
                {
                    _fileManager.DeleteChequeFile(cheque.MetadataFile);
                    _fileManager.DeleteChequeFile(cheque.FrontImageFile);
                    _fileManager.DeleteChequeFile(cheque.RearImageFile);
                });
            }
            catch (Exception ex)
            {
                _logger.Warn($"Error occurred during deletion of cheque files for cheque {cheque.ChequeId}.", ex);
            }
        }

        /// <summary>
        /// Delete cheque files.
        /// </summary>
        /// <param name="cheque">cheque</param>
        private async void DeleteArchiveFiles(Cheque cheque)
        {
            try
            {
                await Retry.Run(x =>
                {
                    _fileManager.DeleteArchiveFile(cheque.MetadataFile);
                    _fileManager.DeleteArchiveFile(cheque.FrontImageFile);
                    _fileManager.DeleteArchiveFile(cheque.RearImageFile);
                });
            }
            catch (Exception ex)
            {
                _logger.Warn($"Error occurred during deletion of archived cheque files for cheque {cheque.ChequeId}.", ex);
            }
        }

        #endregion
    }
}