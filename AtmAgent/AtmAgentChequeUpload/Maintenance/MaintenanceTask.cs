using System;
using System.Threading.Tasks;
using AtmAgentChequeUpload.Config;
using AtmAgentChequeUpload.Files;
using CafeLib.Core.Runnable;

namespace AtmAgentChequeUpload.Maintenance
{
    public class MaintenanceTask : RecurrentTask
    {
        private Func<Task> Callback { get; }

        public MaintenanceTask(IAppConfig config, IChequeFileManager fileManager, Func<Task> task)
            : base(task, GetMaintenanceInterval(config, fileManager), GetMaintenanceStartTime(config, fileManager))
        {
            Callback = task;
            Task = async () => await RunMaintenance(config, fileManager);
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

        private async Task RunMaintenance(IAppConfig config, IChequeFileManager fileManager)
        {
            await Callback();
            var file = new MaintenanceFile(config, fileManager);
            file.SetNextMaintenanceDate();
        }
    }
}
