using Celia.io.Core.StaticObjects.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Celia.io.Core.StaticObjects.Services
{
    public interface IStorageService
    {
        bool ValidStorage(string storageId);

        Task UploadFileAsync(Storage storage, IMediaElement element, Stream stream);

        Task<ServiceAppStorageRelation> FindStorageRelationByIdAsync(
            string appId, string storageId);

        string GetUrlCustom(Storage storage, string filePath, string fileName,
            MediaElementUrlType type, string customStyleProcessStr);

        string GetUrlByStyle(Storage storage, string filePath, string fileName,
            MediaElementUrlType type, string styleName);

        string GetUrlByFormatSizeQuality(Storage storage, string filePath, string fileName,
            MediaElementUrlType type, string format, int maxWidthHeight, int percentage);

        Storage FindStorageById(string storageId);

        Task PublishAsync(Storage storage, ImageElement element);

        Task RevokePublishAsync(Storage storage, IMediaElement element);
    }
}
