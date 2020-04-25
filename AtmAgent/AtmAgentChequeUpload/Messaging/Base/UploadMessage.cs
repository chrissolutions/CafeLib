using System;
using AtmAgent.Cheques;
using CafeLib.Core.Eventing;

namespace AtmAgentChequeUpload.Messaging.Base
{
    public abstract class UploadMessage : EventMessage
    {
        public bool IsSuccessful { get; }

        public int Attempts { get; }

        public Exception Exception { get; }

        public Cheque Cheque { get; }

        protected UploadMessage(Cheque cheque, int attempts)
            : this(cheque, attempts, true, null)
        {
        }

        protected UploadMessage(Cheque cheque, int attempts, Exception ex)
            : this(cheque, attempts, false, ex)
        {
        }

        private UploadMessage(Cheque cheque, int attempts, bool successful, Exception ex)
        {
            Cheque = cheque;
            Attempts = attempts;
            IsSuccessful = successful;
            Exception = ex;
        }
    }
}