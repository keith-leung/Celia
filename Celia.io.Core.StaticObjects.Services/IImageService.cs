using Celia.io.Core.StaticObjects.Abstractions;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Celia.io.Core.StaticObjects.Services
{
    public interface IImageService
    {
        Task<ImageElement> UploadImgAsync(string appId, Stream stream, string storageId, string objectId,
            string extension, string filePath, string srcFileName);

        Task<ImageGroup> FindImgByIdAsync(string objectId);

        Task PublishAsync(string appId, string objectId);

        Task RevokePublishAsync(string appId, string objectId);

        Task<string> GetUrlAsync(string objectId, MediaElementUrlType type, 
            string format, int maxWidthHeight, int percentage);

        Task<string> GetUrlAsync(string objectId, MediaElementUrlType type, string styleName);

        Task<string> GetUrlCustomAsync(string objectId, MediaElementUrlType type, 
            string customStyleProcessStr);

    }
}
