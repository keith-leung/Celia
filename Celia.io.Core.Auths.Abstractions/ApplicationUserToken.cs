using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Celia.io.Core.Auths.Abstractions
{
    [Table("auths_user_tokens")]
    public class ApplicationUserToken : IdentityUserToken<string>
    {
        [Key]
        public string Id { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is ApplicationUserToken))
                return false;

            if (string.IsNullOrEmpty(LoginProvider)
                || string.IsNullOrEmpty(UserId) || string.IsNullOrEmpty(Value))
                return false;

            return LoginProvider.Equals((obj as ApplicationUserToken).LoginProvider)
                && UserId.Equals((obj as ApplicationUserToken).UserId)
                && Value.Equals((obj as ApplicationUserToken).Value);
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        public override string ToString()
        {
            return $"{Id}|{UserId}|{LoginProvider}|{Value}";
        }
    }
}