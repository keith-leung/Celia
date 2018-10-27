using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Celia.io.Core.Auths.Abstractions
{
    [Table("auths_user_roles")]
    public class ApplicationUserRole : IdentityUserRole<string>
    {
        [Key]
        public string Id { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is ApplicationUserRole))
                return false;

            if (string.IsNullOrEmpty(UserId) || string.IsNullOrEmpty(RoleId))
                return false;

            return UserId.Equals((obj as ApplicationUserRole).UserId)
                && RoleId.Equals((obj as ApplicationUserRole).RoleId);
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        public override string ToString()
        {
            return $"{Id}|{UserId}|{RoleId}";
        }
    }
}