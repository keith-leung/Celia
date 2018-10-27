using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Threading.Tasks;
using Celia.io.Core.Auths.Abstractions;

namespace Celia.io.Core.Auths.Services
{
    public class ApplicationUserClaimsPrincipalFactory 
        : UserClaimsPrincipalFactory<
        ApplicationUser, ApplicationRole>
    {  
        public ApplicationUserClaimsPrincipalFactory(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager,
            IOptions<IdentityOptions> optionsAccessor)
            : base(userManager, roleManager, optionsAccessor)
        { 
        }

        //public override async Task<ClaimsPrincipal> CreateAsync(ApplicationUser user)
        //{
        //    ClaimsPrincipal principal = await base.CreateAsync(user);

        //    ((ClaimsIdentity)principal.Identity).AddClaims(new[]
        //    {
        //        new Claim(ClaimTypes.Uri, !string.IsNullOrEmpty(user.PhotoUrl)
        //            ? $"{_appSettings.Domain}/{_appSettings.UploadsFolder}/{user.Id}/{user.PhotoUrl}"
        //            : $"{_appSettings.Domain}/img/default_profile.png")
        //    });

        //    return principal;
        //}
    }
}
