using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using AtmAgent.Cheques;
using AtmAgentChequeUpload.Config;
using AtmAgentChequeUpload.Files;
using AtmAgentChequeUpload.Logging;
using AtmAgentChequeUpload.Messaging;
using CafeLib.Core.Eventing;
using CafeLib.Core.Extensions;
using CafeLib.Core.Runnable;
// ReSharper disable UnusedMember.Global

namespace AtmAgentChequeUpload.Builder
{
    public class ChequeBuilder : IRunnable
    {
        private readonly ConcurrentDictionary<string, ChequeBundle> _bundles = new ConcurrentDictionary<string, ChequeBundle>();
        private readonly IAppConfig _config;
        private readonly IEventService _eventService;
        private readonly IChequeFileManager _fileManager;
        private readonly IChequeParser _parser;
        private readonly ILogger _logger;
        private readonly RecurrentTask _filter;

        public bool IsRunning => _filter.IsRunning;

        public ChequeBuilder(IAppConfig config, IEventService eventService, IChequeFileManager fileManager, IChequeParser parser, ILogger logger)
        {
            _config = config;
            _eventService = eventService;
            _fileManager = fileManager;
            _parser = parser;
            _logger = logger;
            _filter = new RecurrentTask(FilterIncompleteBundles, TimeSpan.FromMinutes(config.TimeOutMinutes), DateTime.Now.Add(TimeSpan.FromMinutes(config.TimeOutMinutes)));
        }

        public Task Start() => _filter.Start();
        public Task Stop() => _filter.Stop();
        public void Dispose() => _filter.Dispose();

        public void AddFile(string fileName)
        {
            if (!IsRunning) return;
            if (!ValidateFile(fileName))
            {
                _logger.Warn($"Ignoring file {fileName}.");
                return;
            }

            _logger.Debug($"Adding file {fileName} to cheque.");
            var chequeFile = _parser.Parse(fileName);
            var bundle = _bundles.GetOrAdd(chequeFile.MicrCode, x => new ChequeBundle(x, chequeFile));
            bundle.AddFile(chequeFile);

            if (!bundle.IsReady) return;
            _logger.Info($"Cheque '{bundle.ChequeId}' has all 3 files. Ready for upload...");
            _eventService.Publish(new ChequeReadyMessage(bundle));
            RemoveCheque(bundle.ChequeId);
        }

        /// <summary>
        /// Add cheque to the bundle map.
        /// </summary>
        /// <param name="cheque"></param>
        public void AddCheque(Cheque cheque)
        {
            if (!IsRunning) return;

            GetChequeFiles(cheque);
            if (ValidateChequeFiles(cheque))
            {
                AddFile(cheque.MetadataFile);
                AddFile(cheque.FrontImageFile);
                AddFile(cheque.RearImageFile);
            }
            else
            {
                _eventService.Publish(new ChequeIncompleteMessage(cheque));
            }
        }

        /// <summary>
        /// Remove cheque from bundle map.
        /// </summary>
        /// <param name="chequeId"></param>
        public void RemoveCheque(string chequeId)
        {
            _bundles.TryRemove(chequeId, out _);
        }

        #region Helpers

        private Task FilterIncompleteBundles()
        {
            var bundles = _bundles
                .Where(x => DateTime.Now > x.Value.LastAccessTime.Add(TimeSpan.FromMinutes(_config.TimeOutMinutes)))
                .Select(x => x.Value)
                .ToArray();

            bundles.ForEach(x =>
            {
                _eventService.Publish(new ChequeIncompleteMessage(x));
                RemoveCheque(x.ChequeId);
            });

            return Task.CompletedTask;
        }

        private bool ValidateFile(string fileName)
        {
            return _fileManager.IsMetadataFile(fileName) || _fileManager.IsImageFile(fileName);
        }

        private void GetChequeFiles(Cheque cheque)
        {
            cheque.MetadataFile = _fileManager.GetMetadataFile(cheque.ChequeId);
            var imageFiles = _fileManager.GetImageFiles(cheque.ChequeId);
            cheque.FrontImageFile = imageFiles.FirstOrDefault();
            cheque.RearImageFile = imageFiles.LastOrDefault();
        }

        private bool ValidateChequeFiles(Cheque cheque)
        {
            var result = true;

            if (!_fileManager.ChequeFileExist(cheque.MetadataFile))
            {
                _logger.Warn($"Cheque {cheque.ChequeId} is missing metadata file {cheque.MetadataFile}.");
                result = false;
            }

            if (!_fileManager.ChequeFileExist(cheque.FrontImageFile))
            {
                _logger.Warn($"Cheque {cheque.ChequeId} is missing cheque front image file {cheque.FrontImageFile}.");
                result = false;
            }

            if (!_fileManager.ChequeFileExist(cheque.RearImageFile))
            {
                _logger.Warn($"Cheque {cheque.ChequeId} is missing cheque rear image file {cheque.RearImageFile}.");
                result = false;
            }

            return result;
        }

        #endregion
    }
}
