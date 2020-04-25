using System;
using AtmAgent.Cheques;
using AtmAgentChequeUpload.Messaging.Base;

namespace AtmAgentChequeUpload.Messaging
{
    public class UploadFailMessage : UploadMessage
    {
        public UploadFailMessage(Cheque cheque, int attempts, Exception ex)
            : base(cheque, attempts, ex)
        {
            Cheque.Status = ChequeStatus.Failed;
        }
    }
}