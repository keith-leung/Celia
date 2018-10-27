using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Celia.io.Core.StaticObjects.Abstractions
{
    [Table("staticobjects_service_apps_storages_relations")]
    public class ServiceAppStorageRelation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [Column("app_id")]
        [MaxLength(100)]
        public string AppId { get; set; }

        [Required]
        [Column("storage_id")]
        [MaxLength(100)]
        public string StorageId { get; set; }

        [Column("store_with_src_file_name")]
        public bool? StoreWithSrcFileName { get; set; }

        [DataType(DataType.DateTime)]
        [Required]
        [Column("ctime")]
        public DateTime CTIME { get; set; }

        [DataType(DataType.DateTime)]
        [Column("utime")]
        public DateTime? UTIME { get; set; }
    }
}
