using System;
using AtmAgent.Cheques;

namespace AtmAgentChequeUpload.Messaging
{
    public class UploadRetryFailMessage : UploadFailMessage
    {
        public UploadRetryFailMessage(Cheque cheque, int attempts, Exception ex)
            : base(cheque, attempts, ex)
        {
        }
    }
}