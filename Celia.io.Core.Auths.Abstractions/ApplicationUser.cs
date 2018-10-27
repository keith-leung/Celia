using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Celia.io.Core.Auths.Abstractions
{
    [Table("auths_users")]
    public class ApplicationUser : IdentityUser<string>
    {
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is ApplicationUser))
                return false;

            return this.Id.ToString().Equals((obj as ApplicationUser).Id);
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        public override string ToString()
        {
            return $"{Id}|{NormalizedUserName}|{UserName}|{NormalizedEmail}|{Email}";
        }
    }
}