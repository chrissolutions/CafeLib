using System;
using System.Linq;
using AtmAgent.Cheques;

namespace AtmAgentChequeUpload.Builder
{
    public class ChequeBundle
    {
        private const int Metadata = 0;
        private const int FrontImage = 1;
        private const int RearImage = 2;

        private readonly ChequeFileInfo[] _files = new ChequeFileInfo[3];

        public string ChequeId { get; }
        public DateTime CreationDate { get; }
        public DateTime LastAccessTime { get; }

        public ChequeBundle(string chequeId, ChequeFileInfo chequeInfo)
        {
            ChequeId = chequeId;
            CreationDate = chequeInfo.FileInfo.CreationTime;
            LastAccessTime = DateTime.Now;
        }

        public void AddFile(ChequeFileInfo file)
        {
            if (ChequeId != file.MicrCode) throw new ArgumentException($"Invalid mismatch: MicrCode {file.MicrCode} does not match check id {ChequeId}.");

            var index = file.IsMetadata
                ? Metadata
                : file.IsFrontPage
                    ? FrontImage
                    : file.IsRearPage
                        ? RearImage
                        : throw new ArgumentException(nameof(file));

            _files[index] = file;
        }

        public bool IsReady => _files[Metadata] != null && _files[FrontImage] != null && _files[RearImage] != null;

        public string MetadataFileName => _files[Metadata]?.FileName;

        public string FrontImageFileName => _files[FrontImage]?.FileName;

        public string RearImageFileName => _files[RearImage]?.FileName;

        public static implicit operator Cheque(ChequeBundle bundle)
        {
            return BuildCheque(bundle);
        }

        private static Cheque BuildCheque(ChequeBundle bundle)
        {
            var cheque = new Cheque
            {
                ChequeId = bundle.ChequeId,
                MetadataFile = bundle.MetadataFileName,
                FrontImageFile = bundle.FrontImageFileName,
                RearImageFile = bundle.RearImageFileName,
                CreationDate = bundle.CreationDate,
                LastUpdateDate = bundle.LastAccessTime
            };

            var file = bundle._files.FirstOrDefault(x => x.DateString != null);
            cheque.ChequeDate = file?.DateString;
            cheque.Atm = file?.AtmId;
            return cheque;
        }
    }
}
