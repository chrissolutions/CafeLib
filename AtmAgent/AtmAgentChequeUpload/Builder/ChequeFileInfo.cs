using System.IO;

namespace AtmAgentChequeUpload.Builder
{
    public class ChequeFileInfo
    {
        public enum ImageFace { Front, Rear }

        public string MicrCode { get; }
        public string DateString { get; }
        public string AtmId { get; }
        public ImageFace? Face { get; }
        public string FileExtension { get; }
        public FileInfo FileInfo { get; }
        public string FileName => FileInfo.Name;

        public bool IsMetadata => Face == null;
        public bool IsImage => Face != null;

        public bool IsFrontPage => IsImage && Face == ImageFace.Front;
        public bool IsRearPage => IsImage && Face == ImageFace.Rear;

        public ChequeFileInfo(FileInfo fileInfo, string micrCode, string dateString, string atmId, ImageFace face, string fileExtension)
        {
            FileInfo = fileInfo;
            MicrCode = micrCode;
            DateString = dateString;
            AtmId = atmId;
            Face = face;
            FileExtension = fileExtension;
        }

        public ChequeFileInfo(FileInfo fileInfo, string micrCode, string dateString, string atmId, string fileExtension)
        {
            FileInfo = fileInfo;
            MicrCode = micrCode;
            DateString = dateString;
            AtmId = atmId;
            FileExtension = fileExtension;
        }
    }
}