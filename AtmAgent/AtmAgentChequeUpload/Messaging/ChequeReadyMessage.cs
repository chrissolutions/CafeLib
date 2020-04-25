using AtmAgent.Cheques;
using AtmAgentChequeUpload.Messaging.Base;

namespace AtmAgentChequeUpload.Messaging
{
    public class ChequeReadyMessage : ChequeStatusMessage
    {
        public ChequeReadyMessage(Cheque cheque)
            : base(cheque)
        {
            cheque.Status = ChequeStatus.Ready;
        }
    }
}
