using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Celia.io.Core.Auths.WebAPI_Core.Models
{
    public class UserSort : SimpleSort
    {
        public string SortBy { get; set; }

        public const string SORT_BY_USERNAME = "UserName";

        public const string SORT_BY_EMAIL = "Email";
    }
}
