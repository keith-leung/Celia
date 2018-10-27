using Celia.io.Core.Auths.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Celia.io.Core.Auths.SDK
{
    public class LoginResponse
    {
        public int StatusCode { get; set; }

        public ApplicationUser User { get; set; }

        public string AccessToken { get; set; }
    }
}
