using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Celia.io.Core.StaticObjects.Abstractions
{
    [Table("staticobjects_storages")]
    public class Storage
    {
        [Key]
        [Column("storage_id")]
        [MaxLength(100)]
        public string StorageId { get; set; }
                
        [Column("storage_name")]
        [MaxLength(100)]
        public string StorageName { get; set; }

        [Required]
        [Column("storage_type")]
        [MaxLength(100)]
        public string StorageType { get; set; }

        [Required]
        [Column("storage_access_key")]
        [MaxLength(1024)]
        public string StorageAccessKey { get; set; }
        
        [Required]
        [Column("download_host")]
        [MaxLength(100)]
        public string DownloadHost { get; set; }
         
        [Column("output_host")]
        [MaxLength(100)]
        public string OutputHost { get; set; }

        [Required]
        [Column("publish_storage_id")]
        [MaxLength(100)]
        public string PublishStorageId { get; set; }

        [Required]
        [Column("publish_storage_access_key")]
        [MaxLength(255)]
        public string PublishStorageAccessKey { get; set; }

        [Required]
        [Column("publish_host")]
        [MaxLength(100)]
        public string PublishHost { get; set; }

        [Column("publish_output_host")]
        [MaxLength(100)]
        public string PublishOutputHost { get; set; }
    }
}
