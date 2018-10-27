using System;
using System.Collections.Generic;
using System.Text;

namespace Celia.io.Core.StaticObjects.Abstractions
{
    public interface IStorageInfo
    {
        string StorageId { get; }

        string StorageAccessKey { get; }

        string StorageHost { get; }

        NetType NetType { get; }
    }
}
