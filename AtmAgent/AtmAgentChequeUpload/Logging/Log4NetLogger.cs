using System;
using System.Reflection;
using log4net;
using log4net.Config;

namespace AtmAgentChequeUpload.Logging
{
    public class Log4NetLogger : ILogger
    {
        private readonly ILog _logger;

        public Log4NetLogger(string loggerName)
        {
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository);
            _logger = LogManager.GetLogger(logRepository.Name, loggerName);

            //XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
            //XmlConfigurator.Configure();
            //_logger = LogManager.GetLogger(Assembly.GetCallingAssembly(), loggerName);
        }

        public void Error(Exception ex)
        {
            _logger.Error(ex);
        }

        public void Warn(string message, Exception ex = null)
        {
            _logger.Warn(message, ex);
        }

        public void Debug(string message)
        {
            _logger.Debug(message);
        }

        public void Info(string message)
        {
            _logger.Info(message);
        }
    }
}