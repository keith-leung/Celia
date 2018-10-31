using System;
using System.Collections.Generic;
using System.Text;

namespace Celia.io.Core.Auths.Abstractions.Exceptions
{
    public class AuthException : Exception
    {
        public AuthException()
        {

        }

        public AuthException(string msg, Exception innerException) : base(msg, innerException)
        {

        }

        public int Code { get; set; }
    }
}
