using System;

namespace Celia.io.Core.StaticFiles.Entities
{
    public class ActionResult
    {
        public bool Succeed { get; set; }
        public int StatusCode { get; set; }
        public string ErrorMessage { get; set; }
        public Exception Cause { get; set; }
    }
}