using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AtmAgentChequeUpload.Config;
using AtmAgentChequeUpload.Logging;

namespace AtmAgentChequeUpload.Files
{
    public class ChequeFileManager : IChequeFileManager
    {
        private readonly IAppConfig _config;
        private readonly ILogger _logger;

        /// <summary>
        /// ChequeFileManager constructor
        /// </summary>
        /// <param name="config">configuration service</param>
        /// <param name="logger">logger service</param>
        public ChequeFileManager(IAppConfig config, ILogger logger)
        {
            _config = config;
            _logger = logger;
        }

        /// <summary>
        /// Get the cheque file data.
        /// </summary>
        /// <param name="fileName">file name</param>
        /// <returns>cheque file data</returns>
        public Task<byte[]> GetChequeFileData(string fileName)
        {
            var tcs = new TaskCompletionSource<byte[]>();
            Task.Run(() =>
            {
                try
                {
                    var fullPath = GetChequePath(fileName);
                    tcs.SetResult(File.ReadAllBytes(fullPath));
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            });

            return tcs.Task;
        }

        /// <summary>
        /// Archive cheque file.
        /// </summary>
        /// <param name="fileName"></param>
        public void ArchiveChequeFile(string fileName)
        {
            if (!ChequeFileExist(fileName)) return;
            var sourceFolder = IsMetadataFile(fileName) ? GetMetadataFolder() : GetImageFolder();
            var sourceFile = Path.Combine(sourceFolder, fileName);
            var destFile = Path.Combine(GetArchiveFolder(), fileName);
            MoveAndOverwriteFile(sourceFile, destFile);
        }

        /// <summary>
        /// Delete archive file.
        /// </summary>
        /// <param name="fileName"></param>
        public void DeleteArchiveFile(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName)) return;
            var archiveFile = Path.Combine(GetArchiveFolder(), fileName);
            if (!File.Exists(archiveFile)) return;
            File.Delete(archiveFile);
        }

        /// <summary>
        /// Determines whether the cheque file exists.
        /// </summary>
        /// <param name="fileName">file name</param>
        /// <returns>true: if the file exist; false otherwise</returns>
        public bool ChequeFileExist(string fileName)
        {
            return !string.IsNullOrWhiteSpace(fileName) && File.Exists(GetChequePath(fileName));
        }

        /// <summary>
        /// Delete cheque file.
        /// </summary>
        /// <param name="fileName"></param>
        public void DeleteChequeFile(string fileName)
        {
            if (!ChequeFileExist(fileName)) return;
            _logger.Warn($"Removing {fileName}.");
            File.Delete(GetChequePath(fileName));
        }

        /// <summary>
        /// Get the cheque path.
        /// </summary>
        /// <param name="fileName">cheque file name</param>
        /// <returns>cheque path corresponding to the file type</returns>
        public string GetChequePath(string fileName)
        {
            var nameOnly = Path.GetFileName(fileName) ?? throw new ArgumentException(nameof(fileName));
            return IsMetadataFile(nameOnly)
                ? Path.Combine(GetMetadataFolder(), nameOnly)
                : IsImageFile(nameOnly)
                    ? Path.Combine(GetImageFolder(), nameOnly)
                    : throw new ArgumentException(nameof(fileName));
        }

        /// <summary>
        /// Get file information.
        /// </summary>
        /// <param name="fileName">file name</param>
        /// <returns>file information</returns>
        public FileInfo GetFileInfo(string fileName)
        {
            return new FileInfo(GetChequePath(fileName));
        }

        /// <summary>
        /// Get the cheque metadata folder.
        /// </summary>
        /// <returns>metadata folder name</returns>
        public string GetMetadataFolder()
        {
            CreateFolder(_config.ChequeMetadataFolderPath);
            return _config.ChequeMetadataFolderPath;
        }

        /// <summary>
        /// Get the cheque image folder.
        /// </summary>
        /// <returns>image folder name</returns>
        public string GetImageFolder()
        {
            CreateFolder(_config.ChequeImageFolderPath);
            return _config.ChequeImageFolderPath;
        }

        /// <summary>
        /// Get the archive folder.
        /// </summary>
        /// <returns>archive folder name</returns>
        public string GetArchiveFolder()
        {
            CreateFolder(_config.ArchiveFolderPath);
            return _config.ArchiveFolderPath;
        }

        /// <summary>
        /// Get the database folder.
        /// </summary>
        /// <returns>database folder name</returns>
        public string GetDatabaseFolder()
        {
            CreateFolder(_config.DatabaseFolderPath);
            return _config.DatabaseFolderPath;
        }

        /// <summary>
        /// Determines whether the cheque file name is a metadata file.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public bool IsMetadataFile(string fileName)
        {
            return Path.GetExtension(fileName)?.ToLower() == _config.MetadataFileExt;
        }

        /// <summary>
        /// Determines whether file name is an image file
        /// </summary>
        /// <param name="fileName">file name</param>
        /// <returns>true: image file; false: otherwise</returns>
        public bool IsImageFile(string fileName)
        {
            return Path.GetExtension(fileName)?.ToLower() == _config.ImageFileExt;
        }

        /// <summary>
        /// Get all metadata files
        /// </summary>
        /// <returns>array of metadata files</returns>
        public string[] GetMetadataFiles()
        {
            var directory = new DirectoryInfo(GetMetadataFolder());
            return directory.GetFiles().Select(x => x.Name).ToArray();
        }

        /// <summary>
        /// Get all image files
        /// </summary>
        /// <returns>array of image files</returns>
        public string[] GetImageFiles()
        {
            var directory = new DirectoryInfo(GetImageFolder());
            return directory.GetFiles().Select(x => x.Name).ToArray();
        }

        /// <summary>
        /// Get cheque metadata file
        /// </summary>
        /// <param name="chequeId">cheque id</param>
        /// <returns>cheque metadata file name</returns>
        public string GetMetadataFile(string chequeId)
        {
            return GetMetadataFiles().FirstOrDefault(x => x.Contains(chequeId));
        }

        /// <summary>
        /// Get the cheque image files
        /// </summary>
        /// <param name="chequeId">cheque id</param>
        /// <returns>array of image files associated with the cheque</returns>
        public string[] GetImageFiles(string chequeId)
        {
            return GetImageFiles().Where(x => x.Contains(chequeId)).ToArray();
        }

        #region Helpers

        /// <summary>
        /// Create folder.
        /// </summary>
        /// <param name="folderPath">path to folder</param>
        private static void CreateFolder(string folderPath)
        {
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
        }

        /// <summary>
        /// We are to first check if file already exist in destination
        /// If the file do exist, we are to delete the file from destination, then move the new copy from source
        /// </summary>
        /// <param name="sourceFile"></param>
        /// <param name="destFile"></param>
        private static void MoveAndOverwriteFile(string sourceFile, string destFile)
        {
            if (File.Exists(destFile))
            {
                File.Delete(destFile);
            }
            File.Move(sourceFile, destFile);
        }

        #endregion
    }
}