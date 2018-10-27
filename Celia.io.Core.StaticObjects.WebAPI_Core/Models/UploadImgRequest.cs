using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BR.StaticObjects.WebAPI_Core.Models
{
    public class UploadImgRequest
    {
        [Required]
        public IFormFile FormFile { get; set; }
   
        public string ObjectId { get; set; }

        [Required]
        public string StorageId { get; set; }
   
        public string FilePath { get; set; }

        public string Extension { get; set; }
    }
}
