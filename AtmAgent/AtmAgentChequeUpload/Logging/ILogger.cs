using System;

namespace AtmAgentChequeUpload.Logging
{
    public interface ILogger
    {
        void Debug(string message);
        void Info(string message);
        void Warn(string message, Exception ex = null);
        void Error(Exception ex);
    }
}