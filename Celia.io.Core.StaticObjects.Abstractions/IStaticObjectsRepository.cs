using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Celia.io.Core.StaticObjects.Abstractions
{
    public interface IStaticObjectsRepository
    {
        Task UpsertElementAsync(ImageElement element);
        ImageElement FindImageElementById(string objectId);
        Task<ImageElementTranItem[]> GetImageTranItemsByObjectIdAsync(string objectId);
        Storage FindStorageById(string storageId);
        Task<ServiceAppStorageRelation> FindStorageRelationByIdAsync(string appId, string storageId);
        Task PublishAsync(ImageElement element);
        Task RevokePublishAsync(ImageElement element);
    }
}
