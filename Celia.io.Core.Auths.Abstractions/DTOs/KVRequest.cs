using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Celia.io.Core.Auths.WebAPI_Core.Models
{
    public class KVRequest
    {
        [Required]
        public string Key { get; set; }

        [Required]
        public string Value { get; set; }
    }
}
