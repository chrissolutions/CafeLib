using AtmAgent.Cheques;
using CafeLib.Core.Eventing;

namespace AtmAgentChequeUpload.Messaging.Base
{
    public abstract class ChequeStatusMessage : EventMessage
    {
        public Cheque Cheque { get; }

        protected ChequeStatusMessage(Cheque cheque)
        {
            Cheque = cheque;
        }
    }
}