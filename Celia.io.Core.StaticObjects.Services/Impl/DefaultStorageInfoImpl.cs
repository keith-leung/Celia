using Celia.io.Core.StaticObjects.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Celia.io.Core.StaticObjects.Services.Impl
{
    class DefaultStorageInfoImpl : IStorageInfo
    {
        public string StorageId { get; set; }

        public string StorageAccessKey { get; set; }

        public string StorageHost { get; set; }

        public NetType NetType { get; set; }
    }
}
