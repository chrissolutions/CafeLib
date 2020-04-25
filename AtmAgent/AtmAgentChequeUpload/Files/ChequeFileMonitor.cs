using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AtmAgentChequeUpload.Logging;
using AtmAgentChequeUpload.Messaging;
using CafeLib.Core.Eventing;
using CafeLib.Core.Extensions;
using CafeLib.Core.Runnable;

namespace AtmAgentChequeUpload.Files
{
    public class ChequeFileMonitor : IRunnable
    {
        private readonly IEventService _eventService;
        private readonly IChequeFileManager _fileManager;
        private readonly ILogger _logger;
        private readonly FileSystemWatcher[] _fileWatchers;

        public bool IsRunning { get; private set; }

        public ChequeFileMonitor(IEventService eventService, IChequeFileManager fileManager, ILogger logger)
        {
            _eventService = eventService;
            _fileManager = fileManager;
            _logger = logger;
            _fileWatchers = new[]
            {
                new FileSystemWatcher(fileManager.GetMetadataFolder()),
                new FileSystemWatcher(fileManager.GetImageFolder())
            };

            _fileWatchers.ForEach(x =>
            {
                x.Created += (s, e) => eventService.Publish(new ChequeFileMessage(e));
                x.EnableRaisingEvents = false;
            });
        }

        /// <summary>
        /// Start the cheque file monitor.
        /// </summary>
        public Task Start()
        {
            if (IsRunning) return Task.CompletedTask;
            _logger.Info($"Start monitoring files in folder: {Environment.NewLine}{string.Join(Environment.NewLine, _fileWatchers.Select(x => x.Path))}");
            _fileManager.GetMetadataFiles().ForEach(x => _eventService.Publish(new ChequeFileMessage(WatcherChangeTypes.All, x)));
            _fileManager.GetImageFiles().ForEach(x => _eventService.Publish(new ChequeFileMessage(WatcherChangeTypes.All, x)));
            _fileWatchers.ForEach(x => x.EnableRaisingEvents = true);
            IsRunning = true;
            return Task.CompletedTask;
        }

        /// <summary>
        /// Stop the cheque file monitor.
        /// </summary>
        public Task Stop()
        {
            if (!IsRunning) return Task.CompletedTask;
            _logger.Info($"Stop monitoring files in folder: {Environment.NewLine}{string.Join(Environment.NewLine, _fileWatchers.Select(x => x.Path))}");
            _fileWatchers.ForEach(x => x.EnableRaisingEvents = false);
            IsRunning = false;
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            Stop();
            _fileWatchers.ForEach(x => x.Dispose());
        }
    }
}