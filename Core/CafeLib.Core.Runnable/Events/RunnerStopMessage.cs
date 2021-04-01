namespace CafeLib.Core.Runnable.Events
{
    public class RunnerStopMessage : RunnerEventMessage
    {
        public RunnerStopMessage(string message)
            : base(message)
        {
        }
    }
}