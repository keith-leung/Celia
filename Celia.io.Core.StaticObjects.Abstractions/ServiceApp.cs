using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Celia.io.Core.StaticObjects.Abstractions
{
    public class ServiceApp
    {
        [Key]
        [Column("app_id")]
        [MaxLength(100)]
        public string AppId { get; set; }
         
        [Column("object_id")]
        [MaxLength(100)]
        [Required]
        public string AppSecret { get; set; }

        [Column("description")]
        public string Description { get; set; } 

        [DataType(DataType.DateTime)]
        [Required]
        [Column("ctime")]
        public DateTime CTIME { get; set; }

        [DataType(DataType.DateTime)]
        [Column("utime")]
        public DateTime? UTIME { get; set; }

        [DataType(DataType.DateTime)]
        [Column("locktime")]
        public DateTime? LockTime { get; set; }
    }
}
