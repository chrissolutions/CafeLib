using System;
using System.Globalization;
using System.IO;
using AtmAgentChequeUpload.Config;
using AtmAgentChequeUpload.Files;
using CafeLib.Core.Extensions;
using CafeLib.Core.Support;
// ReSharper disable UnusedMember.Global

namespace AtmAgentChequeUpload.Maintenance
{
    public class MaintenanceFile
    {
        private const string MaintenanceKey = "maintenanceDate";
        private readonly IAppConfig _config;
        private readonly KeyValueStore _keyValueStore;

        public string FileName { get; }

        public DateTime MaintenanceDate { get; private set; }
        public TimeSpan MaintenanceInterval => _config.DatabaseMaintenanceInterval;

        public MaintenanceFile(IAppConfig config, IChequeFileManager fileManager)
        {
            _config = config;
            FileName = Path.Combine(fileManager.GetDatabaseFolder(), config.MaintenanceFileName);
            _keyValueStore = new KeyValueStore(FileName);
            MaintenanceDate = GetLastMaintenanceDate();
        }

        public void SetNextMaintenanceDate()
        {
            MaintenanceDate = MaintenanceDate.NextTime(_config.DatabaseMaintenanceInterval);
            var dateString = MaintenanceDate.ToString(CultureInfo.InvariantCulture);
            _keyValueStore.AddOrUpdate(MaintenanceKey, x => dateString, (k, v) => dateString);
            _keyValueStore.Write();
        }

        private DateTime GetLastMaintenanceDate()
        {
            if (!_keyValueStore.ContainsKey(MaintenanceKey))
            {
                MaintenanceDate = _config.DatabaseMaintenanceStartTime;
                SetNextMaintenanceDate();
            }

            var value = _keyValueStore[MaintenanceKey];
            return DateTime.Parse(value, CultureInfo.InvariantCulture);
        }
    }
}
