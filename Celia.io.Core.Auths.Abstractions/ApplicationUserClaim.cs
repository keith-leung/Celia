using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Celia.io.Core.Auths.Abstractions
{
    [Table("auths_user_claims")]
    public class ApplicationUserClaim : IdentityUserClaim<string>
    {
        [Key]
        public new string Id { get; set; }

        public override string ToString()
        {
            return $"{Id}|{ClaimType}|{ClaimValue}";
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is ApplicationUserClaim))
                return false;
            if (string.IsNullOrEmpty(ClaimType) || string.IsNullOrEmpty(ClaimValue))
                return false;

            return ClaimType.Equals((obj as ApplicationRoleClaim).ClaimType)
                && ClaimValue.Equals((obj as ApplicationRoleClaim).ClaimValue);
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }
    }
}