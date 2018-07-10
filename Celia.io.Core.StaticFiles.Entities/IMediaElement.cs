using System;

namespace Celia.io.Core.StaticFiles.Entities
{
    public interface IMediaElement
    {
        string Id { get; set; }
        MediaElementType ElementType { get; set; }
        bool IsDeleted { get; set; }
        DateTime CTime { get; set; }
        DateTime? UTime { get; set; }
        string PartitionId { get; set; }
        string Path { get; set; }
        string SrcFileNameWithoutExt { get; set; }
        string Extension { get; set; }
    }
}