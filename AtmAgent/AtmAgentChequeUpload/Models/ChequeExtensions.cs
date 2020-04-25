// ReSharper disable UnusedMember.Global

using AtmAgent.Cheques;

namespace AtmAgentChequeUpload.Models
{
    public static class ChequeExtensions
    {
        public static bool IsEmpty(this Cheque cheque)
        {
            return string.IsNullOrWhiteSpace(cheque.MetadataFile)
                   && string.IsNullOrWhiteSpace(cheque.FrontImageFile)
                   && string.IsNullOrWhiteSpace(cheque.RearImageFile);
        }
    }
}
