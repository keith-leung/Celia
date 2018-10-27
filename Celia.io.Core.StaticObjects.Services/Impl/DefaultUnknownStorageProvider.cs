using Celia.io.Core.StaticObjects.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Celia.io.Core.StaticObjects.Services.Impl
{
    class DefaultUnknownStorageProvider : IStorageProvider
    {
        public Stream GetStream(string storageId, string storageAccessKey, StorageMode mode,
            string downloadHost, string filePath, string fileName)
        {
            return null;
        }

        public string GetUrlByFormatSizeQuality(IStorageInfo storageInfo, MediaElementUrlType urlType, 
            string filePath, string fileName, string format, int maxWidthHeight, int percentage)
        {
            return string.Empty;
        }

        public string GetUrlByStyle(IStorageInfo storageInfo, MediaElementUrlType urlType, 
            string filePath, string fileName, string styleName)
        {
            return string.Empty;
        }

        public string GetUrlCustom(IStorageInfo storageInfo, MediaElementUrlType urlType, 
            string filePath, string fileName, string customStyleProcessStr)
        {
            return string.Empty;
        }

        public async Task RemoveFileAsync(string storageId, string storageAccessKey, StorageMode mode,
            string downloadHost, string filePath, string fileName)
        {
        }

        public async Task UploadFileAsync(Stream stream, string storageId, string storageAccessKey,
             StorageMode mode, string downloadHost, string filePath, string fileName)
        {
        }
    }
}
