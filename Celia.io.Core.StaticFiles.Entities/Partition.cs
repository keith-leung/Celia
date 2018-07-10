using System;

namespace Celia.io.Core.StaticFiles.Entities
{
    public class Partition : IPartition
    {
        public string Id { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CTime { get; set; }
        public DateTime? UTime { get; set; }
        public virtual PartitionType Type { get; set; }
        public virtual PartitionVendor Vendor { get; set; }
    }
}