using Caomei.Core.Models;
using System.IO;

namespace Caomei.Core.Support.FileHandlers
{
    public interface IWtmFileHandler
    {
        (string path, string handlerInfo) Upload(string fileName, long fileLength, Stream data, string group = null, string subdir = null, string extra = null);

        Stream GetFileData(IWtmFile file);

        void DeleteFile(IWtmFile file);
    }
}