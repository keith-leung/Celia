using Celia.io.Core.Auths.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;

namespace Celia.io.Core.Auths.Services
{
    public class ApplicationUserManager : AspNetUserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store, 
            IOptions<IdentityOptions> optionsAccessor, 
            IPasswordHasher<ApplicationUser> passwordHasher,
            IEnumerable<IUserValidator<ApplicationUser>> userValidators, 
            IEnumerable<IPasswordValidator<ApplicationUser>> passwordValidators,
            ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors,
            IServiceProvider services, ILogger<AspNetUserManager<ApplicationUser>> logger)
            : base(store, optionsAccessor, passwordHasher, userValidators, 
                  passwordValidators, keyNormalizer, errors, services, logger)
        {
        }
    }
}
