using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BR.StaticObjects.WebAPI_Core.Models
{
    public class MediaElementActionRequest
    {
        [Required]
        public string ObjectId { get; set; }
    }
}
