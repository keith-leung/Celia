using System;
using System.Collections.Generic;
using System.Text;

namespace Celia.io.Core.StaticObjects.Abstractions.DTOs
{
    public class LockoutUserRequest
    {
        public string UserId { get; set; }

        public DateTime LockoutEndTimeUtc { get; set; }
    }
}
