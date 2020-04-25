using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using AtmAgentChequeUpload.Models;
using CafeLib.Core.Extensions;
using Microsoft.Rest;
using Newtonsoft.Json.Linq;

// ReSharper disable UnusedMember.Global

namespace AtmAgentChequeUpload.Logging
{
    internal class TraceLogger : IServiceClientTracingInterceptor
    {
        private readonly ILogger _logger;
        private const string Prefix = "TRACE - ";

        public TraceLogger(ILogger logger)
        {
            _logger = logger;
        }

        public void Configuration(string source, string name, string value)
        {
            throw new NotImplementedException();
        }

        public void EnterMethod(string invocationId, object instance, string method, IDictionary<string, object> parameters)
        {
            var dto = (DocumentUploadDto)parameters.First().Value;
            var details = JObject.FromObject(dto.DocumentMetadata).Children().First().Children().First().Children().First().Children().First();
            var micr = details["MICR"].Value<string>().Replace("<", "").Replace(" ", "").Replace("=", "").Replace(":", "");

            _logger.Debug($"{Prefix}Enter with invocationId: {invocationId}, ChequeId: {micr}");
            _logger.Debug($"{Prefix}{nameof(dto.ChannelTypeCode)}: {dto.ChannelTypeCode}");
            _logger.Debug($"{Prefix}{nameof(dto.DocumentProcessTypeCode)}: {dto.DocumentProcessTypeCode}");
            _logger.Debug($"{Prefix}{nameof(dto.DocumentTypeCode)}: {dto.DocumentTypeCode}");
            _logger.Debug($"{Prefix}{nameof(dto.StaffNumber)}: {dto.StaffNumber}");
            _logger.Debug($"{Prefix}{nameof(dto.DocumentMetadata)}: {dto.DocumentMetadata.AsFormattedString()}");
            _logger.Debug($"{Prefix}{nameof(dto.ContentElements)}: {dto.ContentElements?.Count ?? 0}");
        }

        public void ExitMethod(string invocationId, object returnValue)
        {
            _logger.Debug($"{Prefix}Exit with invocationId: {invocationId}, return value: {returnValue ?? string.Empty}");
        }

        public void Information(string message)
        {
            _logger.Info($"{Prefix}{message}");
        }

        public void ReceiveResponse(string invocationId, HttpResponseMessage response)
        {
            _logger.Debug($"{Prefix}Response invocationId: {invocationId}");
            _logger.Debug($"{Prefix}Response Status code: {response.StatusCode.Humanize()}");
            _logger.Debug($"{Prefix}Response Description: {response.ReasonPhrase}");
            _logger.Debug($"{Prefix}Response Content: {response.Content}");
        }

        public void SendRequest(string invocationId, HttpRequestMessage request)
        {
            _logger.Debug($"{Prefix}Request invocationId: {invocationId}");
            _logger.Debug($"{Prefix}Request Uri: {request.RequestUri}");
            _logger.Debug($"{Prefix}Request Method: {request.Method.Method}");
            _logger.Debug($"{Prefix}Request Content Length: {request.Content.ReadAsByteArrayAsync().Result?.Length ?? 0}");
        }

        public void TraceError(string invocationId, Exception exception)
        {
            _logger.Warn($"{Prefix}trace error in: {invocationId}", exception);
        }
    }
}
