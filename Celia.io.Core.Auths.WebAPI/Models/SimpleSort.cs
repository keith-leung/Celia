using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Celia.io.Core.Auths.WebAPI_Core.Models
{
    public class SimpleSort
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int PageIndex { get; set; }

        [Range(1, 2000)]
        public int? PageSize { get; set; }

        [Range(0, 1)]
        public int? OrderType { get; set; }
    }
}
