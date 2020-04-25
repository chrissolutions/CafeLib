using System;
using System.IO;
using AtmAgentChequeUpload.Config;
using AtmAgentChequeUpload.Files;

namespace AtmAgentChequeUpload.Builder
{
    public class ChequeParser : IChequeParser
    {
        private readonly IAppConfig _config;
        private readonly IChequeFileManager _fileManager;

        public ChequeParser(IAppConfig config, IChequeFileManager fileManager)
        {
            _config = config;
            _fileManager = fileManager;
        }

        /// <summary>
        /// CHQ_440461123055024098800_20180725_A3031007_f.tif 
        /// 440461123055024098800_20180725_A3031007.xml
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public ChequeFileInfo Parse(string fileName)
        {
            var extension = Path.GetExtension(fileName)?.ToLower();
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName) ?? string.Empty;

            var tokens = fileNameWithoutExtension.Split('_');
            switch (tokens.Length)
            {
                case 5 when extension == _config.ImageFileExt && tokens[0].Equals("CHQ", StringComparison.InvariantCultureIgnoreCase):
                {
                    var face = tokens[4] == "f" ? ChequeFileInfo.ImageFace.Front : ChequeFileInfo.ImageFace.Rear;
                    return new ChequeFileInfo(_fileManager.GetFileInfo(fileName), tokens[1], tokens[2], tokens[3], face, extension);
                }

                case 3 when extension == _config.MetadataFileExt:
                    return new ChequeFileInfo(_fileManager.GetFileInfo(fileName), tokens[0], tokens[1], tokens[2], extension);

                default:
                    return null;
            }
        }
    }
}
