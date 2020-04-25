using AtmAgent.Cheques;
using CafeLib.Core.Queueing;

namespace AtmAgentChequeUpload.Uploading
{
    public class UploadChequeQueue : QueueProducer<Cheque>
    {
        public UploadChequeQueue(IUploadCheque queueConsumer) 
            : base(queueConsumer)
        {
        }
    }
}