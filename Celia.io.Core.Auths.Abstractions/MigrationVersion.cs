using System;

namespace Celia.io.Core.Auths.Abstractions
{
    [System.ComponentModel.DataAnnotations.Schema.Table("__migrationversions")]
    public class MigrationVersion
    {
        [System.ComponentModel.DataAnnotations.Key]
        public string VersionCode { get; set; }
        public string Version { get; set; }
        public DateTime CTIME { get; set; }
    }
}