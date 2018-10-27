using Celia.io.Core.Auths.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Celia.io.Core.Auths.WebAPI_Core.Models
{
    public class LoginResponse
    {
        public int StatusCode { get; set; }

        public ApplicationUser User { get; set; }

        public string AccessToken { get; set; }
    }
}
