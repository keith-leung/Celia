using Celia.io.Core.StaticObjects.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

namespace BR.StaticObjects.DataAccess
{
    public class EfCoreStaticObjectsRepository : IStaticObjectsRepository
    {
        private StaticObjectsDbContext _dbContext;

        public EfCoreStaticObjectsRepository(StaticObjectsDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public ImageElement FindImageElementById(string objectId)
        {
            return this._dbContext.ImageElements.Find(objectId);
        }

        public Storage FindStorageById(string storageId)
        {
            return this._dbContext.Storages.Find(storageId);
        }

        public async Task<ServiceAppStorageRelation> FindStorageRelationByIdAsync(string appId, string storageId)
        {
            var result = this._dbContext.ServiceAppStorageRelations.FirstOrDefault(
                m => m.AppId.Equals(appId, StringComparison.InvariantCultureIgnoreCase)
                && m.StorageId.Equals(storageId, StringComparison.InvariantCultureIgnoreCase));

            return result;
        }

        public async Task<ImageElementTranItem[]> GetImageTranItemsByObjectIdAsync(string objectId)
        {
            var result = this._dbContext.ImageElementTranItems.Where(
                m => m.ObjectId.Equals(objectId, StringComparison.InvariantCultureIgnoreCase));

            return result.ToArray();
        }

        public Task PublishAsync(ImageElement element)
        {
            return this._dbContext.ImageElements.FindAsync(element.ObjectId)
                .ContinueWith(m2 =>
                {
                    if (!m2.IsFaulted && m2.Result != null)
                    {
                        m2.Result.UTIME = DateTime.Now;
                        m2.Result.IsPublished = true;

                        _dbContext.Update(m2.Result);
                        _dbContext.SaveChanges();
                    }
                });
        }

        public Task RevokePublishAsync(ImageElement element)
        {
            return this._dbContext.ImageElements.FindAsync(element.ObjectId)
                .ContinueWith(m2 =>
                {
                    if (!m2.IsFaulted && m2.Result != null)
                    {
                        m2.Result.UTIME = DateTime.Now;
                        m2.Result.IsPublished = false;

                        _dbContext.Update(m2.Result);
                        _dbContext.SaveChanges();
                    }
                });
        }

        public Task UpsertElementAsync(ImageElement element)
        {
            return this._dbContext.ImageElements.FindAsync(element.ObjectId)
                .ContinueWith(m2 =>
                {
                    if (!m2.IsFaulted && m2.Result != null)
                    {
                        m2.Result.UTIME = DateTime.Now;
                        m2.Result.StoreWithSrcFileName = element.StoreWithSrcFileName;
                        m2.Result.StorageId = element.StorageId;
                        m2.Result.SrcFileName = element.SrcFileName;
                        m2.Result.IsPublished = element.IsPublished;
                        m2.Result.FilePath = element.FilePath;
                        m2.Result.Extension = element.Extension;

                        _dbContext.Update(m2.Result);
                        _dbContext.SaveChanges();
                    }
                    else if (!m2.IsFaulted)
                    {
                        ImageElement entity = new ImageElement()
                        {
                            ObjectId = element.ObjectId,
                            CTIME = DateTime.Now,
                            Extension = element.Extension,
                            FilePath = element.FilePath,
                            IsPublished = element.IsPublished,
                            SrcFileName = element.SrcFileName,
                            StorageId = element.StorageId,
                            StoreWithSrcFileName = element.StoreWithSrcFileName,
                        };

                        _dbContext.ImageElements.Add(entity);
                        _dbContext.SaveChanges();
                    }
                });
        }
    }
}
