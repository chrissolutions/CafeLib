using System;
using AtmAgentChequeUpload.Maintenance;

namespace AtmAgentChequeUpload.Config
{
    public interface IAppConfig
    {
        string ChequeImageFolderPath { get; }
        string ChequeMetadataFolderPath { get; }
        string ArchiveFolderPath { get; }

        string DatabaseFolderPath { get; }
        string DatabaseFileName { get; }
        string MaintenanceFileName { get; }

        string MetadataFileExt { get; }
        string ImageFileExt { get; }

        string DocumentReceiverApiUrl { get; }

        int TimeOutMinutes { get; }

        int MaxRetryAttempts { get; }

        int ArchiveRetryMinutes { get; }

        string DatabaseMaintenancePeriod { get; }
        TimeSpan DatabaseMaintenanceInterval { get; }
        DateTime DatabaseMaintenanceStartTime { get; }
        MaintenanceMode DatabaseMaintenanceMode { get; }

        string DocumentProcessTypeCode { get; }
        string ChannelTypeCode { get; }
        string DocumentTypeCode { get; }

        string UserName { get; }
        string Password { get; }
        string ConsumerPrivateKey { get; }
        Guid EnvironmentId { get; }
    }
}