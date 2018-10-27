using System;
using System.Collections.Generic;
using System.Text;

namespace Celia.io.Core.StaticObjects.Abstractions
{
    [Flags]
    public enum MediaElementUrlType
    {
        Unknown = 0,
        DownloadUrl = 1,
        OutputUrl = 2,
        PublishDownloadUrl = 4,
        PublishOutputUrl = 8,
    }
}
