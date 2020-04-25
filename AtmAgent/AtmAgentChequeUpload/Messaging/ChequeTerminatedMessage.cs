using System;
using AtmAgent.Cheques;
using AtmAgentChequeUpload.Messaging.Base;

namespace AtmAgentChequeUpload.Messaging
{
    public abstract class ChequeTerminatedMessage : ChequeStatusMessage
    {
       public ChequeStatus Status { get; }

        protected ChequeTerminatedMessage(Cheque cheque)
            : base(cheque)
        {
            Status = cheque.Status;
            cheque.LastUpdateDate = DateTime.Now;
        }
    }
}
