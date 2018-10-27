using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Celia.io.Core.StaticObjects.Abstractions
{
    [Table("staticobjects_image_elements")]
    public class ImageElement : IMediaElement
    {
        [Key]
        [Column("object_id")]
        [MaxLength(100)]
        public string ObjectId { get; set; }

        [Column("src_file_name")]
        [MaxLength(100)]
        public string SrcFileName { get; set; }

        [Required]
        [Column("storage_id")]
        [MaxLength(100)]
        public string StorageId { get; set; }

        [Column("extension")]
        [MaxLength(100)]
        public string Extension { get; set; }

        [Column("store_with_src_file_name")]
        public bool StoreWithSrcFileName { get; set; }

        [Column("file_path")]
        [MaxLength(1024)]
        public string FilePath { get; set; }

        [Column("is_published")]
        public bool IsPublished { get; set; }

        [DataType(DataType.DateTime)]
        [Required]
        [Column("ctime")]
        public DateTime CTIME { get; set; }

        [DataType(DataType.DateTime)]
        [Column("utime")]
        public DateTime? UTIME { get; set; }

        public string GetFullPath()
        {
            string filePath = string.Empty;
            if(!string.IsNullOrEmpty(this.FilePath) &&
                !string.IsNullOrEmpty(this.FilePath?.Trim(new char[] { '/', '\\' })))
            {
                filePath = this.FilePath.Trim(new char[] { '/', '\\' }) + "/" + this.GetFileName();
                return filePath;
            }
            else {
                return this.GetFileName();
            }
            //return $"{this.FilePath?.Trim(new char[] { '/', '\\' })}/{this.GetFileName()}";
            //return System.IO.Path.Combine(this.FilePath, this.GetFileName());
        }

        public string GetFileName()
        {
            return this.StoreWithSrcFileName ? this.SrcFileName
                : $"{this.ObjectId}.{this.Extension}";
        }
    }
}
