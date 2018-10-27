using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Celia.io.Core.Auths.Abstractions;
using Celia.io.Core.Auths.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Celia.io.Core.Auths.WebAPI_Core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserRolesController : ControllerBase
    {
        private readonly ILogger<UserRolesController> _logger;
        private readonly ApplicationUserManager _userManager;
        private readonly ApplicationRoleManager _roleManager;

        public UserRolesController(ILogger<UserRolesController> logger,
            ApplicationUserManager userManager,
            ApplicationRoleManager roleManager)
        {
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            this._roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
        }

        // GET: api/UserRoles
        [HttpPost("adduserroles")]
        public async Task<IEnumerable<ApplicationUserRole>> AddUserRoles(
            [FromBody] IEnumerable<ApplicationUserRole> userRoles)
        {
            if (userRoles == null || userRoles.Count() < 1)
                return new ApplicationUserRole[] { };

            ApplicationUser user = new ApplicationUser()
            {
                Id = userRoles.First().UserId
            };
            IEnumerable<string> roles = from one in userRoles
                                        select one.RoleId;

            var result = await _userManager.AddToRolesAsync(user, roles);
            if (result.Succeeded)
            {
                return userRoles;
            }

            return null;
        }

        // POST: api/UserRoles
        [HttpPost("adduserrole")]
        public async Task<ApplicationUserRole> AddUserRole(
            [FromBody] ApplicationUserRole userRole)
        {
            ApplicationUser user = new ApplicationUser()
            {
                Id = userRole.UserId
            };

            var role = await this._roleManager.FindByIdAsync(userRole.RoleId);
            if (role != null)
            {
                var result = await _userManager.AddToRoleAsync(user, role.Name);
                if (result.Succeeded)
                {
                    return userRole;
                }
            }

            return null;
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("removeuserrole")]
        public async Task RemoveUserRole([FromBody] ApplicationUserRole userRole)
        {
            ApplicationUser user = new ApplicationUser()
            {
                Id = userRole.UserId
            };

            var role = await this._roleManager.FindByIdAsync(userRole.RoleId);
            if (role != null)
            {
                await _userManager.RemoveFromRoleAsync(user, role.Name);
            }
        }

        // GET: api/UserRoles
        [HttpPost("removeuserroles")]
        public async Task RemoveUserRoles(
            [FromBody] IEnumerable<ApplicationUserRole> userRoles)
        {
            if (userRoles == null || userRoles.Count() < 1)
                return;

            ApplicationUser user = new ApplicationUser()
            {
                Id = userRoles.First().UserId
            };
            IEnumerable<string> roles = from one in userRoles
                                        select one.RoleId;

            await _userManager.RemoveFromRolesAsync(user, roles);
        }

        [HttpGet("isinrole")]
        public async Task<bool> IsInRole([FromQuery] [Required] string userId,
            [FromQuery] [Required] string roleName)
        {
            return await _userManager.IsInRoleAsync(new ApplicationUser() { Id = userId }, roleName);
        }
    }
}
