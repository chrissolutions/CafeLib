using System;
using System.Configuration;
using System.Globalization;
using AtmAgentChequeUpload.Maintenance;
// ReSharper disable UnusedMember.Global

namespace AtmAgentChequeUpload.Config
{
    public class AppConfig : IAppConfig
    {
        public string ChequeImageFolderPath { get; }
        public string ChequeMetadataFolderPath { get; }
        public string ArchiveFolderPath { get; }
        public string DocumentReceiverApiUrl { get; }
        public int TimeOutMinutes { get; }
        public int MaxRetryAttempts { get; }
        public int ArchiveRetryMinutes { get; }
        public string UserName { get; }
        public string Password { get; }
        public string ConsumerPrivateKey { get; }
        public Guid EnvironmentId { get; }
        public string DocumentProcessTypeCode { get; }
        public string ChannelTypeCode { get; }
        public string DocumentTypeCode { get; }

        public string DatabaseFolderPath { get; }
        public string DatabaseFileName { get; }
        public string MaintenanceFileName { get; }

        public string MetadataFileExt { get; }
        public string ImageFileExt { get; }

        public string DatabaseMaintenancePeriod { get; }
        public TimeSpan DatabaseMaintenanceInterval { get; }
        public DateTime DatabaseMaintenanceStartTime { get; }
        public MaintenanceMode DatabaseMaintenanceMode { get; }

        public AppConfig()
        {
            ConfigurationManager.RefreshSection("appSettings");
            ChequeImageFolderPath = ConfigurationManager.AppSettings[nameof(ChequeImageFolderPath)];
            ChequeMetadataFolderPath = ConfigurationManager.AppSettings[nameof(ChequeMetadataFolderPath)];
            ArchiveFolderPath = ConfigurationManager.AppSettings[nameof(ArchiveFolderPath)];
            DatabaseFolderPath = ConfigurationManager.AppSettings[nameof(DatabaseFolderPath)];
            DatabaseFileName = ConfigurationManager.AppSettings[nameof(DatabaseFileName)];
            MaintenanceFileName = ConfigurationManager.AppSettings[nameof(MaintenanceFileName)];

            MetadataFileExt = ConfigurationManager.AppSettings[nameof(MetadataFileExt)];
            ImageFileExt = ConfigurationManager.AppSettings[nameof(ImageFileExt)];

            DatabaseMaintenancePeriod = ConfigurationManager.AppSettings[nameof(DatabaseMaintenancePeriod)] ?? "fortnightly";
            DatabaseMaintenanceStartTime = GetDateTime(nameof(DatabaseMaintenanceStartTime), "H:mm");
            DatabaseMaintenanceInterval = GetTimeSpan(DatabaseMaintenancePeriod);
            DatabaseMaintenanceMode = GetMaintenanceMode(ConfigurationManager.AppSettings[nameof(DatabaseMaintenanceMode)]);

            ArchiveRetryMinutes = GetInt(nameof(ArchiveRetryMinutes), defaultTo: 10);
            MaxRetryAttempts = GetInt(nameof(MaxRetryAttempts), defaultTo: 10);
            TimeOutMinutes = GetInt(nameof(TimeOutMinutes), defaultTo: 10);
            ArchiveRetryMinutes = GetInt(nameof(ArchiveRetryMinutes), defaultTo: 10);

            DocumentReceiverApiUrl = ConfigurationManager.AppSettings[nameof(DocumentReceiverApiUrl)];
            DocumentProcessTypeCode = ConfigurationManager.AppSettings[nameof(DocumentProcessTypeCode)];
            ChannelTypeCode = ConfigurationManager.AppSettings[nameof(ChannelTypeCode)];
            DocumentTypeCode = ConfigurationManager.AppSettings[nameof(DocumentTypeCode)];

            UserName = ConfigurationManager.AppSettings[nameof(UserName)];
            Password = ConfigurationManager.AppSettings[nameof(Password)];
            ConsumerPrivateKey = ConfigurationManager.AppSettings[nameof(ConsumerPrivateKey)];
            EnvironmentId = GetGuid(nameof(EnvironmentId));
        }

        private static int GetInt(string key, int defaultTo = 0)
        {
            var setting = ConfigurationManager.AppSettings[key];
            if (string.IsNullOrEmpty(setting))
            {
                return defaultTo;
            }

            if (int.TryParse(setting, out var value))
            {
                return value;
            }

            throw new Exception($"Invalid value '{setting}' for config key '{key}'");
        }

        // ReSharper disable once UnusedMember.Local
        private static bool GetBool(string key, bool defaultTo = false)
        {
            var setting = ConfigurationManager.AppSettings[key];
            if (string.IsNullOrEmpty(setting))
            {
                return defaultTo;
            }

            if (bool.TryParse(setting, out var value))
            {
                return value;
            }

            throw new Exception($"Invalid value '{setting}' for config key '{key}'");
        }

        private static Guid GetGuid(string key)
        {
            var setting = ConfigurationManager.AppSettings[key];
            if (string.IsNullOrEmpty(setting))
            {
                throw new Exception($"App.Config does not have setting {key}");
            }

            if (Guid.TryParse(setting, out var value))
            {
                return value;
            }

            throw new Exception($"Invalid value '{setting}' for config key '{key}'");
        }

        private static DateTime GetDateTime(string key, string format)
        {
            var setting = ConfigurationManager.AppSettings[key];
            if (string.IsNullOrEmpty(setting)) return DateTime.Now;

            return DateTime.TryParseExact(setting, format, null, DateTimeStyles.None, out var dt)
                ? dt
                : DateTime.Now;
        }

        private static TimeSpan GetTimeSpan(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return TimeSpan.Zero;

            var split = input.Trim().ToLowerInvariant().Split(' ');
            var number = int.TryParse(split[0], out var i) ? i : 1;
            var timeFrame = split.Length == 2 ? split[1] : split[0];

            switch (timeFrame.ToLowerInvariant().Trim())
            {
                case "monthly":
                    return new TimeSpan(30, 0, 0, 0);

                case "fortnightly":
                    return new TimeSpan(14, 0, 0, 0);

                case "weekly":
                    return new TimeSpan(7, 0, 0, 0);

                case "daily":
                    return new TimeSpan(1, 0, 0, 0);

                case "hourly":
                    return new TimeSpan(0, 1, 0, 0);

                case "day":
                case "days":
                    return new TimeSpan(number, 0, 0, 0);

                case "hour":
                case "hours":
                    return new TimeSpan(0, number, 0, 0);

                case "minute":
                case "minutes":
                    return new TimeSpan(0, 0, number, 0);

                case "second":
                case "seconds":
                    return new TimeSpan(0, 0, 0, number);

                default:
                    if (int.TryParse(timeFrame, out var milliseconds))
                    {
                        return new TimeSpan(0, 0, 0, 0, milliseconds);
                    }
                    else if (TimeSpan.TryParse(timeFrame, out var ts))
                    {
                        return ts;
                    }

                    throw new ArgumentException($"Invalid time frame value {timeFrame}.");
            }
        }

        private MaintenanceMode GetMaintenanceMode(string mode)
        {
            if (string.IsNullOrWhiteSpace(mode)) return MaintenanceMode.Default;

            switch (mode.ToLowerInvariant().Trim())
            {
                case "default":
                    return MaintenanceMode.Default;

                case "remove":
                    return MaintenanceMode.Remove;

                case "retain":
                    return MaintenanceMode.Retain;

                default:
                    return MaintenanceMode.Default;
            }
        }
    }
}