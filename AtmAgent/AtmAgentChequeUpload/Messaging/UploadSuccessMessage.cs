using AtmAgent.Cheques;
using AtmAgentChequeUpload.Messaging.Base;

namespace AtmAgentChequeUpload.Messaging
{
    public class UploadSuccessMessage : UploadMessage
    {
        public UploadSuccessMessage(Cheque cheque, int retry = 1)
            : base(cheque, retry)
        {
            Cheque.Status = ChequeStatus.Uploaded;
        }
    }
}