using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Celia.io.Core.StaticObjects.Abstractions;
using Microsoft.Extensions.Logging;

namespace Celia.io.Core.StaticObjects.Services.Impl
{
    public class StorageService : IStorageService
    {
        private readonly ILogger<StorageService> _logger;
        private readonly IStaticObjectsRepository _repository;
        private readonly IServiceProvider _serviceProvider;

        public StorageService(ILogger<StorageService> logger, IStaticObjectsRepository repository,
            IServiceProvider serviceProvider)
        {
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._repository = repository ?? throw new ArgumentNullException(nameof(repository));
            this._serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public Storage FindStorageById(string storageId)
        {
            return this._repository.FindStorageById(storageId);
        }

        public async Task<ServiceAppStorageRelation> FindStorageRelationByIdAsync(
            string appId, string storageId)
        {
            return await this._repository.FindStorageRelationByIdAsync(appId, storageId);
        }

        public bool ValidStorage(string storageId)
        {
            var storage = this._repository.FindStorageById(storageId);
            return (storage != null);
        }

        public string GetUrlByFormatSizeQuality(Storage storage, string filePath, string fileName,
            MediaElementUrlType type, string format, int maxWidthHeight, int percentage)
        {
            IStorageProvider storageProvider = StorageProviderFactory.GetStorageProvider(
                _serviceProvider, storage.StorageType);
            switch (type)
            {
                case MediaElementUrlType.DownloadUrl:
                    {
                        return storageProvider.GetUrlByFormatSizeQuality(
                            new DefaultStorageInfoImpl()
                            {
                                NetType = NetType.Intranet,
                                StorageAccessKey = storage.StorageAccessKey,
                                StorageId = storage.StorageId,
                                StorageHost = storage.DownloadHost,
                            }, type, filePath, fileName, format,
                            maxWidthHeight, percentage);
                    } 
                case MediaElementUrlType.OutputUrl:
                    {
                        return storageProvider.GetUrlByFormatSizeQuality(
                            new DefaultStorageInfoImpl()
                            {
                                NetType = NetType.Internet,
                                StorageAccessKey = storage.StorageAccessKey,
                                StorageId = storage.StorageId,
                                StorageHost = storage.OutputHost,
                            }, type, filePath, fileName, format,
                            maxWidthHeight, percentage);
                    }
                case MediaElementUrlType.PublishDownloadUrl:
                    {
                        return storageProvider.GetUrlByFormatSizeQuality(
                            new DefaultStorageInfoImpl()
                            {
                                NetType = NetType.Intranet,
                                StorageAccessKey = storage.PublishStorageAccessKey,
                                StorageId = storage.PublishStorageId,
                                StorageHost = storage.PublishOutputHost,
                            }, type, filePath, fileName, format,
                            maxWidthHeight, percentage);
                    }
                default:
                    {
                        return storageProvider.GetUrlByFormatSizeQuality(
                            new DefaultStorageInfoImpl()
                            {
                                NetType = NetType.Internet,
                                StorageAccessKey = storage.PublishStorageAccessKey,
                                StorageId = storage.PublishStorageId,
                                StorageHost = storage.PublishOutputHost,
                            }, type, filePath, fileName, format,
                            maxWidthHeight, percentage);
                    }
            } 
        }

        public string GetUrlByStyle(Storage storage, string filePath, string fileName,
            MediaElementUrlType type, string styleName)
        {
            IStorageProvider storageProvider = StorageProviderFactory.GetStorageProvider(
                _serviceProvider, storage.StorageType);

            switch (type)
            {
                case MediaElementUrlType.DownloadUrl:
                    {
                        return storageProvider.GetUrlByStyle(
                            new DefaultStorageInfoImpl()
                            {
                                NetType = NetType.Intranet,
                                StorageAccessKey = storage.StorageAccessKey,
                                StorageId = storage.StorageId,
                                StorageHost = storage.DownloadHost,
                            }, type, filePath, fileName, styleName);
                    }
                case MediaElementUrlType.OutputUrl:
                    {
                        return storageProvider.GetUrlByStyle(
                            new DefaultStorageInfoImpl()
                            {
                                NetType = NetType.Internet,
                                StorageAccessKey = storage.StorageAccessKey,
                                StorageId = storage.StorageId,
                                StorageHost = storage.OutputHost,
                            }, type, filePath, fileName, styleName);
                    }
                case MediaElementUrlType.PublishDownloadUrl:
                    {
                        return storageProvider.GetUrlByStyle(
                            new DefaultStorageInfoImpl()
                            {
                                NetType = NetType.Intranet,
                                StorageAccessKey = storage.PublishStorageAccessKey,
                                StorageId = storage.PublishStorageId,
                                StorageHost = storage.PublishOutputHost,
                            }, type, filePath, fileName, styleName);
                    }
                default:
                    {
                        return storageProvider.GetUrlByStyle(
                            new DefaultStorageInfoImpl()
                            {
                                NetType = NetType.Internet,
                                StorageAccessKey = storage.PublishStorageAccessKey,
                                StorageId = storage.PublishStorageId,
                                StorageHost = storage.PublishOutputHost,
                            }, type, filePath, fileName, styleName);
                    }
            } 
        }

        public string GetUrlCustom(Storage storage, string filePath, string fileName,
            MediaElementUrlType type, string customStyleProcessStr)
        {
            IStorageProvider storageProvider = StorageProviderFactory.GetStorageProvider(
                _serviceProvider, storage.StorageType);

            switch (type)
            {
                case MediaElementUrlType.DownloadUrl:
                    {
                        return storageProvider.GetUrlCustom(
                            new DefaultStorageInfoImpl()
                            {
                                NetType = NetType.Intranet,
                                StorageAccessKey = storage.StorageAccessKey,
                                StorageId = storage.StorageId,
                                StorageHost = storage.DownloadHost,
                            }, type, filePath, fileName, customStyleProcessStr);
                    }
                case MediaElementUrlType.OutputUrl:
                    {
                        return storageProvider.GetUrlCustom(
                            new DefaultStorageInfoImpl()
                            {
                                NetType = NetType.Internet,
                                StorageAccessKey = storage.StorageAccessKey,
                                StorageId = storage.StorageId,
                                StorageHost = storage.OutputHost,
                            }, type, filePath, fileName, customStyleProcessStr);
                    }
                case MediaElementUrlType.PublishDownloadUrl:
                    {
                        return storageProvider.GetUrlCustom(
                            new DefaultStorageInfoImpl()
                            {
                                NetType = NetType.Intranet,
                                StorageAccessKey = storage.PublishStorageAccessKey,
                                StorageId = storage.PublishStorageId,
                                StorageHost = storage.PublishOutputHost,
                            }, type, filePath, fileName, customStyleProcessStr);
                    }
                default:
                    {
                        return storageProvider.GetUrlCustom(
                            new DefaultStorageInfoImpl()
                            {
                                NetType = NetType.Internet,
                                StorageAccessKey = storage.PublishStorageAccessKey,
                                StorageId = storage.PublishStorageId,
                                StorageHost = storage.PublishOutputHost,
                            }, type, filePath, fileName, customStyleProcessStr);
                    }
            } 
        }

        public async Task PublishAsync(Storage storage, ImageElement element)
        {
            IStorageProvider storageProvider = StorageProviderFactory.GetStorageProvider(
                _serviceProvider, storage.StorageType);

            using (Stream stream = storageProvider.GetStream(
                    storage.StorageId, storage.StorageAccessKey, StorageMode.Internal,
                    storage.DownloadHost, element.FilePath, element.GetFileName()))
            {
                await storageProvider.UploadFileAsync(
                    stream, storage.PublishStorageId, storage.PublishStorageAccessKey,
                    StorageMode.External, storage.PublishHost, element.FilePath, element.GetFileName());
            }
        }

        public async Task RevokePublishAsync(Storage storage, IMediaElement element)
        {
            IStorageProvider storageProvider = StorageProviderFactory.GetStorageProvider(
                _serviceProvider, storage.StorageType);

            await storageProvider.RemoveFileAsync(storage.PublishStorageId,
                storage.PublishStorageAccessKey, StorageMode.Internal,
                storage.PublishHost, element.FilePath, element.GetFileName());
        }

        public async Task UploadFileAsync(Storage storage, IMediaElement element, Stream stream)
        {
            IStorageProvider storageProvider = StorageProviderFactory.GetStorageProvider(
                _serviceProvider, storage.StorageType);

            await storageProvider.UploadFileAsync(stream, storage.StorageId,
                storage.StorageAccessKey, StorageMode.Internal, storage.DownloadHost,
                element.FilePath, element.GetFileName());
        }
    }
}
