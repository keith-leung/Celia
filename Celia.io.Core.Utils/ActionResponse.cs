using System;
using System.Collections.Generic;
using System.Text;

namespace Celia.io.Core.Utils
{
    public class ActionResponse<T>
    {
        public int Status { get; set; }
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }

        public T Data { get; set; }
    }
}
