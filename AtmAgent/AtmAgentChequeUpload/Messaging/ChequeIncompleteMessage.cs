using AtmAgent.Cheques;
using AtmAgentChequeUpload.Messaging.Base;

namespace AtmAgentChequeUpload.Messaging
{
    public class ChequeIncompleteMessage : ChequeStatusMessage
    {
        public ChequeIncompleteMessage(Cheque cheque)
            : base(cheque)
        {
            cheque.Status = ChequeStatus.Incomplete;
        }
    }
}
