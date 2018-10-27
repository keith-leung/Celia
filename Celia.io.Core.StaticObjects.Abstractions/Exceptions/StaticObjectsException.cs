using System;
using System.Collections.Generic;
using System.Text;

namespace Celia.io.Core.StaticObjects.Abstractions.Exceptions
{
    public class StaticObjectsException : Exception
    {
        public StaticObjectsException()
        {

        }

        public StaticObjectsException(string msg, Exception innerException) 
            : base(msg, innerException)
        {

        }

        public int Code { get; set; }
    }
}
