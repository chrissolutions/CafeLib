using AtmAgent.Cheques;

namespace AtmAgentChequeUpload.Messaging
{
    public class ChequeExpiryMessage : ChequeTerminatedMessage
    {
        public ChequeExpiryMessage(Cheque cheque)
            : base(cheque)
        {
            cheque.Status = ChequeStatus.Expired;
        }
    }
}
