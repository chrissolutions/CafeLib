using System;

namespace AtmAgentChequeUpload.Logging
{
    public class ConsoleLogger : ILogger
    {
        public void Debug(string message)
        {
            Console.WriteLine(message);
        }

        public void Info(string message)
        {
            Console.WriteLine(message);
        }

        public void Error(Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(ex);
            Console.ResetColor();
        }

        public void Warn(string message, Exception ex = null)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(ex == null ? message : $"{message} Exception: {ex}");
            Console.ResetColor();
        }
    }
}