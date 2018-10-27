using Celia.io.Core.StaticObjects.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Celia.io.Core.StaticObjects.Services
{
    public interface IStorageProvider
    {
        string GetUrlByFormatSizeQuality(IStorageInfo storageInfo, MediaElementUrlType urlType, 
            string filePath, string fileName, string format,
            int maxWidthHeight, int percentage);

        string GetUrlByStyle(IStorageInfo storageInfo, MediaElementUrlType urlType, 
            string filePath, string fileName, string styleName);

        string GetUrlCustom(IStorageInfo storageInfo, MediaElementUrlType urlType, 
            string filePath, string fileName, string customStyleProcessStr);

        Task UploadFileAsync(Stream stream, string storageId, string storageAccessKey,
            StorageMode mode, string downloadHost, string filePath, string fileName);

        Stream GetStream(string storageId, string storageAccessKey,
            StorageMode mode, string downloadHost, string filePath, string fileName);

        Task RemoveFileAsync(string storageId, string storageAccessKey,
            StorageMode mode, string host, string filePath, string fileName); 
    }
}
