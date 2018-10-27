using System;
using System.Collections.Generic;
using System.Text;
using Celia.io.Core.Auths.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Celia.io.Core.Auths.Services
{
    public class ApplicationRoleManager : AspNetRoleManager<ApplicationRole>
    {
        public ApplicationRoleManager(IRoleStore<ApplicationRole> store,
            IEnumerable<IRoleValidator<ApplicationRole>> roleValidators,
            ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors,
            ILogger<AspNetRoleManager<ApplicationRole>> logger,
            IHttpContextAccessor contextAccessor)
            : base(store, roleValidators, keyNormalizer,
                  errors, logger, contextAccessor)
        {

        }
    }
}
