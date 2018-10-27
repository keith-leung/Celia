using System;

namespace Celia.io.Core.StaticObjects.Abstractions
{
    public interface IMediaElement
    {
        DateTime CTIME { get; set; }
        string Extension { get; set; }
        string FilePath { get; set; }
        bool IsPublished { get; set; }
        string ObjectId { get; set; }
        string SrcFileName { get; set; }
        string StorageId { get; set; }
        bool StoreWithSrcFileName { get; set; }
        DateTime? UTIME { get; set; }

        string GetFileName();
        string GetFullPath();
    }
}