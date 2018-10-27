using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Celia.io.Core.Auths.WebAPI_Core.Models
{
    public class TokenResponse
    {
        public bool IsValid { get; set; }
        public bool IsRefreshRequired { get; set; }
    }
}
