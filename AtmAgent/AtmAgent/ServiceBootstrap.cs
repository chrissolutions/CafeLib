using System;
using AtmAgentChequeUpload.Builder;
using AtmAgentChequeUpload.Config;
using AtmAgentChequeUpload.Controller;
using AtmAgentChequeUpload.Data;
using AtmAgentChequeUpload.Files;
using AtmAgentChequeUpload.Logging;
using CafeLib.Core.IoC;
using CafeLib.Data;

// ReSharper disable UnusedMember.Global

namespace AtmAgent
{
    public static class ServiceBootstrap
    {
        /// <summary>
        /// Bootstrap the service 
        /// </summary>
        /// <typeparam name="T">IServiceProcess</typeparam>
        public static IServiceRegistry Bootstrap<T>() where T : class, IServiceController
        {
            var serviceRegistry = IocFactory.CreateRegistry();
            serviceRegistry.AddEventService();
            serviceRegistry.AddSingleton<IAppConfig, AppConfig>();
            serviceRegistry.AddSingleton(x => GetLogger());
            serviceRegistry.AddSingleton<IChequeFileManager, ChequeFileManager>();
            serviceRegistry.AddSingleton<IChequeParser, ChequeParser>();
            serviceRegistry.AddSingleton<IDatabase, ChequeDatabase>();
            serviceRegistry.AddSingleton<T>();
            return serviceRegistry;
        }

        private static ILogger GetLogger()
        {
            return (Environment.UserInteractive)
                ? (ILogger)new ConsoleLogger()
                : new Log4NetLogger("ATMChequeUploadAgentLogger");
        }
    }
}