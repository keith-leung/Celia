using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Celia.io.Core.StaticObjects.Abstractions;
using Celia.io.Core.StaticObjects.Abstractions.Exceptions;
using Microsoft.Extensions.Logging;

namespace Celia.io.Core.StaticObjects.Services.Impl
{
    public class ImageService : IImageService
    {
        private readonly ILogger<ImageService> _logger;
        private readonly IStorageService _storageService;
        private readonly IServiceAppService _serviceAppService;
        private readonly IStaticObjectsRepository _repository;

        public ImageService(ILogger<ImageService> logger,
            IServiceAppService serviceAppService,
            IStorageService storageService, IStaticObjectsRepository repository)
        {
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._storageService = storageService ?? throw new ArgumentNullException(nameof(storageService));
            this._serviceAppService = serviceAppService ?? throw new ArgumentNullException(nameof(serviceAppService));
            this._repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<ImageGroup> FindImgByIdAsync(string objectId)
        {
            ImageElement element = _repository.FindImageElementById(objectId);
            if (element == null)
            {
                return null;
            }

            ImageElementTranItem[] tranItems =
                await _repository.GetImageTranItemsByObjectIdAsync(objectId);

            return new ImageGroup()
            {
                MediaElement = element,
                TramItems = tranItems,
            };
        }

        public async Task<string> GetUrlAsync(string objectId, MediaElementUrlType type,
            string format, int maxWidthHeight, int percentage)
        {
            ImageElement element = _repository.FindImageElementById(objectId);
            if (element == null)
                return string.Empty;

            Storage storage = _storageService.FindStorageById(element.StorageId);
            if (storage == null)
                return string.Empty;

            string url = _storageService.GetUrlByFormatSizeQuality(
                storage, element.FilePath, element.GetFileName(),
                type, format, maxWidthHeight, percentage);

            return url;
        }

        public async Task<string> GetUrlAsync(string objectId, MediaElementUrlType type,
            string styleName)
        {
            ImageElement element = _repository.FindImageElementById(objectId);
            if (element == null)
                return string.Empty;

            Storage storage = _storageService.FindStorageById(element.StorageId);
            if (storage == null)
                return string.Empty;

            string url = _storageService.GetUrlByStyle(storage,
                element.FilePath, element.GetFileName(), type, styleName);

            return url;
        }

        public async Task<string> GetUrlCustomAsync(string objectId, MediaElementUrlType type,
            string customStyleProcessStr)
        {
            ImageElement element = _repository.FindImageElementById(objectId);
            if (element == null)
                return string.Empty;

            Storage storage = _storageService.FindStorageById(element.StorageId);
            if (storage == null)
                return string.Empty;

            string url = _storageService.GetUrlCustom(
                storage, element.FilePath, element.GetFileName(), type, customStyleProcessStr);

            return url;
        }

        public async Task PublishAsync(string appId, string objectId)
        {
            ImageElement element = _repository.FindImageElementById(objectId);
            if (element == null)
                return;
            ServiceAppStorageRelation rel = await _storageService.FindStorageRelationByIdAsync(
                appId, element.StorageId);

            if (rel == null)
            {
                throw new StaticObjectsException(
                    $"APPID {appId} is not allowed to use STORAGE {element.StorageId}",
                    null)
                {
                    Code = (int)System.Net.HttpStatusCode.MethodNotAllowed,
                };
            }

            Storage storage = _storageService.FindStorageById(element.StorageId);

            await _storageService.PublishAsync(storage, element);

            await _repository.PublishAsync(element);
        }

        public async Task RevokePublishAsync(string appId, string objectId)
        {
            ImageElement element = _repository.FindImageElementById(objectId);
            if (element == null)
                return;
            ServiceAppStorageRelation rel = await _storageService.FindStorageRelationByIdAsync(
                appId, element.StorageId);

            if (rel == null)
            {
                throw new StaticObjectsException(
                    $"APPID {appId} is not allowed to use STORAGE {element.StorageId}",
                    null)
                {
                    Code = (int)System.Net.HttpStatusCode.MethodNotAllowed,
                };
            }

            Storage storage = _storageService.FindStorageById(element.StorageId);

            await _storageService.RevokePublishAsync(storage, element);

            await _repository.RevokePublishAsync(element);
        }

        public async Task<ImageElement> UploadImgAsync(string appId, Stream stream, string storageId,
            string objectId, string extension, string filePath, string srcFileName)
        {
            ServiceAppStorageRelation rel = await _storageService.FindStorageRelationByIdAsync(
                appId, storageId);

            if (rel == null)
            {
                throw new StaticObjectsException(
                    $"APPID {appId} is not allowed to use STORAGE {storageId}",
                    null)
                {
                    Code = (int)System.Net.HttpStatusCode.MethodNotAllowed,
                };
            }

            ImageElement element = new ImageElement()
            {
                StorageId = storageId,
                Extension = extension,
                CTIME = DateTime.Now,
                FilePath = filePath,
                IsPublished = false,
                ObjectId = objectId,
                SrcFileName = srcFileName,
                StoreWithSrcFileName = rel.StoreWithSrcFileName.GetValueOrDefault(),
            };

            Storage storage = _storageService.FindStorageById(element.StorageId);

            await _storageService.UploadFileAsync(storage, element, stream);

            await _repository.UpsertElementAsync(element);

            return element;
        }
    }
}
