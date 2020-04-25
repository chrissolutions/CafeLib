using System;
using System.IO;
using AtmAgentChequeUpload.Files;
using CafeLib.Core.Eventing;

namespace AtmAgentChequeUpload.Messaging
{
    public class ChequeFileMessage : EventMessage
    {
        public ChequeFileChange Change { get; }

        public string FileName { get; }

        public ChequeFileMessage(WatcherChangeTypes change, string fileName)
        {
            Change = ToChequeFileChange(change);
            FileName = Path.GetFileName(fileName);
        }

        public ChequeFileMessage(FileSystemEventArgs e)
            : this(e.ChangeType, e.FullPath)
        {
        }

        private ChequeFileChange ToChequeFileChange(WatcherChangeTypes change)
        {
            switch (change)
            {
                case WatcherChangeTypes.All:
                    return ChequeFileChange.Found;

                case WatcherChangeTypes.Created:
                    return ChequeFileChange.Created;

                case WatcherChangeTypes.Deleted:
                    return ChequeFileChange.Deleted;

                case WatcherChangeTypes.Changed:
                    return ChequeFileChange.Changed;

                case WatcherChangeTypes.Renamed:
                    return ChequeFileChange.Renamed;

                default:
                    throw new ArgumentException(nameof(change));
            }
        }
    }
}
