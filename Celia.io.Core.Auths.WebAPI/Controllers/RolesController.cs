using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Celia.io.Core.Auths.Abstractions;
using Celia.io.Core.Auths.Services;
using Celia.io.Core.Auths.WebAPI_Core.Models;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections;

namespace Celia.io.Core.Auths.WebAPI_Core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly ILogger<RolesController> _logger;
        private readonly ApplicationRoleManager _roleManager;

        public RolesController(ILogger<RolesController> logger, ApplicationRoleManager roleManager)
        {
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
        }

        [HttpGet("test")]
        public async Task<string> test([FromQuery] string tester)
        {
            return $"{tester} action received. ";
        }

        // GET: api/Roles/list
        [HttpGet("list")]
        public async Task<IEnumerable<ApplicationRole>> List([FromQuery] SimpleSort simpleSort)
        {
            int pageSize2 = 20;
            if (simpleSort.PageSize.HasValue)
                pageSize2 = simpleSort.PageSize.Value;

            if (simpleSort.OrderType.HasValue && simpleSort.OrderType.Value != 0)
            {
                return this._roleManager.Roles.OrderByDescending(m => m.NormalizedName)
                    .Skip((simpleSort.PageIndex - 1) * pageSize2).Take(pageSize2);
            }

            return this._roleManager.Roles.OrderBy(m => m.NormalizedName)
                .Skip((simpleSort.PageIndex - 1) * pageSize2).Take(pageSize2);
        }

        // GET: api/Roles/findbyid/{id}
        [HttpGet("findbyid")]
        public async Task<ApplicationRole> FindById([FromQuery] [Required] string roleId)
        {
            return await this._roleManager.FindByIdAsync(roleId);
        }

        [HttpGet("findbyname")]
        public async Task<ApplicationRole> FindByName([FromQuery] [Required] string roleName)
        {
            return await this._roleManager.FindByNameAsync(roleName);
        }

        [HttpPost("create")]
        public async Task<ApplicationRole> Create([FromBody] ApplicationRole role)
        {
            var result = await this._roleManager.CreateAsync(role);
            if (result.Succeeded)
                return role;

            return null;
        }

        [HttpPost("update")]
        public async Task Update([FromBody] ApplicationRole role)
        {
            await this._roleManager.UpdateAsync(role);
        }

        [HttpPost("delete")]
        public async Task Delete([FromBody] ApplicationRole role)
        {
            await this._roleManager.DeleteAsync(role);
        }

        [HttpPost("addclaimstorole")]
        public async Task<IEnumerable<ApplicationRoleClaim>> AddClaimsToRole(
            [FromBody] IEnumerable<ApplicationRoleClaim> roleClaims)
        {
            foreach (var rc in roleClaims)
            {
                await _roleManager.AddClaimAsync(
                    new ApplicationRole()
                    {
                        Id = rc.RoleId
                    },
                    new System.Security.Claims.Claim(
                        rc.ClaimType,
                        rc.ClaimValue));
            }

            return roleClaims;
        }

        [HttpGet("getclaimsbyroleid")]
        public async Task<IEnumerable<ApplicationRoleClaim>> GetClaimsByRoleId([FromQuery] [Required] string roleId)
        {
            ApplicationRole role = await _roleManager.FindByIdAsync(roleId);
            if (role != null)
            {
                var result = await _roleManager.GetClaimsAsync(role);
                return (from one in result
                        select new ApplicationRoleClaim()
                        {
                            RoleId = roleId,
                            ClaimType = one.Type,
                            ClaimValue = one.Value
                        });
            }

            return null;
        }
    }
}
