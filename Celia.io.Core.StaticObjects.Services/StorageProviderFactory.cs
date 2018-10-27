using Celia.io.Core.Utils;
using Celia.io.Core.StaticObjects.Services.Impl;
using System;
using System.Collections.Generic;
using System.Text;

namespace Celia.io.Core.StaticObjects.Services
{
    public class StorageProviderFactory
    {
        public static IStorageProvider GetStorageProvider(
            IServiceProvider serviceProvider, string storageType)
        {
            var disconfService = serviceProvider.GetService(typeof(DisconfService)) as DisconfService;
            object typeObject = null;
            disconfService.CustomConfigs.TryGetValue(
                $"STORAGE_PROVIDER_TYPE_{storageType}".ToUpperInvariant(), out typeObject);
            if (typeObject != null && typeObject is Type)
            {
                Type storageProviderType = typeObject as Type;
                var providerObject = serviceProvider.GetService(storageProviderType);
                return providerObject as IStorageProvider;
            }

            return new DefaultUnknownStorageProvider();
        }
    }
}
