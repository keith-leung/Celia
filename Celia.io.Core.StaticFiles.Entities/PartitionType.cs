using System;

namespace Celia.io.Core.StaticFiles.Entities
{
    [Flags]
    public enum PartitionType
    {
        Default = 0,
        Image = 1,
        Audio = 2,
        Video = 4,
    }
}