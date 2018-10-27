using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Celia.io.Core.Auths.Abstractions
{
    [Table("auths_roles")]
    public class ApplicationRole : IdentityRole<string>
    {
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is ApplicationRole))
                return false;

            return this.ToString().Equals(obj.ToString());
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        public override string ToString()
        {
            return $"{Id}|{NormalizedName}|{Name}";
        }
    }
}