using AtmAgent.Cheques;
using CafeLib.Core.Queueing;

namespace AtmAgentChequeUpload.Uploading
{
    public interface IUploadCheque : IQueueConsumer<Cheque>
    {
    }
}