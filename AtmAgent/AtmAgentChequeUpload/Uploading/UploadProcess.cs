using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;
using AtmAgent.Cheques;
using AtmAgentChequeUpload.Config;
using AtmAgentChequeUpload.Files;
using AtmAgentChequeUpload.Logging;
using AtmAgentChequeUpload.Messaging;
using AtmAgentChequeUpload.Models;
using CafeLib.Core.Eventing;
using CafeLib.Core.Support;
using Microsoft.Rest;
using Newtonsoft.Json;
// ReSharper disable UnusedMember.Global

namespace AtmAgentChequeUpload.Uploading
{
    public class UploadProcess : IUploadCheque
    {
        private readonly IAppConfig _config;
        private readonly IEventService _eventService;
        private readonly IChequeFileManager _fileManager;
        private readonly ILogger _logger;

        //private readonly IDocumentReceiver _documentReceiverProxy;
        //private readonly IProxyHeaderContentProvider _proxyHeaderProvider;

        public UploadProcess(IAppConfig config, IEventService eventService, IChequeFileManager fileManager, ILogger logger)
        {
            _config = config;
            _eventService = eventService;
            _fileManager = fileManager;
            _logger = logger;

            //_documentReceiverProxy = new DocumentReceiver();
            //_proxyHeaderProvider = new MyProxyHeaderProvider(config);
#if TRACE
            ServiceClientTracing.AddTracingInterceptor(new TraceLogger(logger));
#endif
        }

        public async Task Consume(Cheque item)
        {
            try
            {
                await Retry.Run(_config.MaxRetryAttempts, async x =>
                {
                    var tcs = new TaskCompletionSource<bool>();
                    try
                    {
                        _logger.Debug($"Uploading attempt {x} for cheque {item.ChequeId} ...");
                        var headers = new Dictionary<string, List<string>>(); //await _proxyHeaderProvider.CreateCustomHeaders();
                        var dto = await CreateDocumentUploadDto(item);
                        await UploadDocument(item, x, _config.DocumentReceiverApiUrl, headers, dto);
                        tcs.SetResult(true);
                    }
                    catch (Exception ex)
                    {
                        tcs.SetException(ex);
                    }

                    return await tcs.Task;
                });
            }
            catch (Exception ex)
            {
                _eventService.Publish(new UploadRetryFailMessage(item, _config.MaxRetryAttempts, ex));
            }
        }

        private async Task UploadDocument(Cheque cheque, int retry, string uri, Dictionary<string, List<string>> headers, DocumentUploadDto dto)
        {
            try
            {
                ServiceClientTracing.IsEnabled = true;
                await Task.CompletedTask; // Set FAKE compiler variable to fake successful upload when the next line is commented out.
                //#if FAKE_UPLOAD
                //await Task.CompletedTask; // Set FAKE compiler variable to fake successful upload when the next line is commented out.
                //#else 
                //await _documentReceiverProxy.Documents.UploadDocumentAsync(uri, headers, dto);
                //#endif
                _eventService.Publish(new UploadSuccessMessage(cheque, retry));
            }
            catch (Exception ex)
            {
                _eventService.Publish(new UploadFailMessage(cheque, retry, ex));
                throw;
            }
            finally
            {
                ServiceClientTracing.IsEnabled = false;
            }
        }

        private async Task<DocumentUploadDto> CreateDocumentUploadDto(Cheque cheque)
        {
            var metadata = await GetMetadata(cheque.MetadataFile);

            var dto = new DocumentUploadDto
            {
                DocumentProcessTypeCode = _config.DocumentProcessTypeCode,
                ChannelTypeCode = _config.ChannelTypeCode,
                DocumentTypeCode = _config.DocumentTypeCode,
                DocumentMetadata = metadata,
                StaffNumber = 0,
                ContentElements = new List<ContentElement>()
            };

            var front = await CreateContentElement(cheque.FrontImageFile);
            dto.ContentElements.Add(front);

            var rear = await CreateContentElement(cheque.RearImageFile);
            dto.ContentElements.Add(rear);

            return dto;
        }

        private Task<IDictionary<string, object>> GetMetadata(string fileName)
        {
            var doc = new XmlDocument();
            doc.Load(_fileManager.GetChequePath(fileName));
            var json = JsonConvert.SerializeXmlNode(doc);
            return Task.FromResult(JsonConvert.DeserializeObject<IDictionary<string, object>>(json));
        }

        private async Task<ContentElement> CreateContentElement(string fileName)
        {
            var file = new ContentElement { Name = fileName, ContentType = "image/tiff" };
            var data = await GetFileData(fileName);
            file.ContentSize = data.Length;
            file.Content = Convert.ToBase64String(data);
            return file;
        }

        private async Task<byte[]> GetFileData(string fileName)
        {
            return await _fileManager.GetChequeFileData(fileName);
        }
    }
}
