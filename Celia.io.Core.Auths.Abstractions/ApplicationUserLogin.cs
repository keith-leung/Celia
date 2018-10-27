using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Celia.io.Core.Auths.Abstractions
{
    [Table("auths_user_logins")]
    public class ApplicationUserLogin : IdentityUserLogin<string>
    {
        [Key]
        public string Id { get; set; }

        public override string ToString()
        {
            return $"{Id}|{ProviderKey}|{LoginProvider}";
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is ApplicationUserLogin))
                return false;
            if (string.IsNullOrEmpty(LoginProvider)
                || string.IsNullOrEmpty(this.ProviderKey))
                return false;

            return LoginProvider.Equals((obj as ApplicationUserLogin).LoginProvider)
                && ProviderKey.Equals((obj as ApplicationUserLogin).ProviderKey);
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }
    }
}