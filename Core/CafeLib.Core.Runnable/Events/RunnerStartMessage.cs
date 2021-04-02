namespace CafeLib.Core.Runnable.Events
{
    public class RunnerStartMessage : RunnerEventMessage
    {
        public RunnerStartMessage(string message)
            : base(message)
        {
        }
    }
}