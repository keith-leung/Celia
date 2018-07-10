using System;

namespace Celia.io.Core.StaticFiles.Entities
{
    public interface IPartition
    {
        string Id { get; set; }
        bool IsDeleted { get; set; }
        DateTime CTime { get; set; }
        DateTime? UTime { get; set; }

        PartitionType Type { get; set; }
        PartitionVendor Vendor { get; set; }
    }
}