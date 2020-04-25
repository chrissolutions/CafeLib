using AtmAgent.Cheques;

namespace AtmAgentChequeUpload.Messaging
{
    public class ChequeCancelledMessage : ChequeTerminatedMessage
    {
        public ChequeCancelledMessage(Cheque cheque)
            : base(cheque)
        {
            cheque.Status = ChequeStatus.Cancelled;
        }
    }
}
