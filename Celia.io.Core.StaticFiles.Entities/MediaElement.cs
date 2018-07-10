using System;

namespace Celia.io.Core.StaticFiles.Entities
{
    public class MediaElement : IMediaElement
    {
        public string Id { get; set; }
        public MediaElementType ElementType { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CTime { get; set; }
        public DateTime? UTime { get; set; }
        public string PartitionId { get; set; }
        public virtual string Path { get; set; }
        public virtual string SrcFileNameWithoutExt { get; set; }
        public virtual string Extension { get; set; }
    }
}