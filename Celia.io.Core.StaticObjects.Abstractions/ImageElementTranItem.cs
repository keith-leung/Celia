using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Celia.io.Core.StaticObjects.Abstractions
{
    [Table("staticobjects_image_element_tran_items")]
    public class ImageElementTranItem
    {
        [Key]
        [Column("object_id")]
        [MaxLength(100)]
        public string ObjectId { get; set; }

        [Required]
        [Column("image_id")]
        [MaxLength(100)]
        public string ImageId { get; set; }
        
        [Column("extension")]
        [MaxLength(100)]
        public string Extension { get; set; }

        [Column("tran_item_type")]
        [MaxLength(100)]
        public string TranItemType { get; set; }

        [Column("file_path")]
        [MaxLength(1024)]
        public string FilePath { get; set; }

        [DataType(DataType.DateTime)]
        [Required]
        [Column("ctime")]
        public DateTime CTIME { get; set; }

        [DataType(DataType.DateTime)] 
        [Column("utime")]
        public DateTime? UTIME { get; set; }
    }
}
