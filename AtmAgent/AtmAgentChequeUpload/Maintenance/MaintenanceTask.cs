using System;
using System.Threading.Tasks;
using AtmAgentChequeUpload.Config;
using AtmAgentChequeUpload.Files;
using CafeLib.Core.Runnable;

namespace AtmAgentChequeUpload.Maintenance
{
    public class MaintenanceTask : RecurrentTask
    {
        public MaintenanceTask(IAppConfig config, IChequeFileManager fileManager, Func<Task> task)
            : base(async () => await RunMaintenance(config, fileManager, task), GetMaintenanceInterval(config, fileManager), GetMaintenanceStartTime(config, fileManager))
        {
        }

        private static TimeSpan GetMaintenanceInterval(IAppConfig config, IChequeFileManager fileManager)
        {
            var file = new MaintenanceFile(config, fileManager);
            return file.MaintenanceInterval;
        }

        private static DateTime GetMaintenanceStartTime(IAppConfig config, IChequeFileManager fileManager)
        {
            var file = new MaintenanceFile(config, fileManager);
            return file.MaintenanceDate > DateTime.Now ? file.MaintenanceDate : default;
        }

        private static async Task RunMaintenance(IAppConfig config, IChequeFileManager fileManager, Func<Task> task)
        {
            await task();
            var file = new MaintenanceFile(config, fileManager);
            file.SetNextMaintenanceDate();
        }
    }
}
