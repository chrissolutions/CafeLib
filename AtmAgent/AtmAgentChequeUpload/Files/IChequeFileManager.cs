using System.IO;
using System.Threading.Tasks;

namespace AtmAgentChequeUpload.Files
{
    public interface IChequeFileManager
    {
        Task<byte[]> GetChequeFileData(string fileName);
        string GetChequePath(string fileName);
        FileInfo GetFileInfo(string fileName);

        void ArchiveChequeFile(string fileName);
        void DeleteArchiveFile(string fileName);

        bool ChequeFileExist(string fileName);
        void DeleteChequeFile(string fileName);

        bool IsMetadataFile(string fileName);
        bool IsImageFile(string fileName);

        string GetMetadataFolder();
        string GetImageFolder();
        string GetArchiveFolder();
        string GetDatabaseFolder();

        string[] GetMetadataFiles();
        string[] GetImageFiles();

        string GetMetadataFile(string chequeId);
        string[] GetImageFiles(string chequeId);
    }
}