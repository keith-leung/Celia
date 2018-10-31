using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Celia.io.Core.Auths.Abstractions;
using Celia.io.Core.Auths.Services;
using Celia.io.Core.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Celia.io.Core.Auths.WebAPI_Core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleClaimsController : ControllerBase
    {
        private readonly ILogger<RoleClaimsController> _logger;
        private readonly ApplicationRoleManager _roleManager;

        public RoleClaimsController(ILogger<RoleClaimsController> logger, ApplicationRoleManager roleManager)
        {
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
        }

        // GET: api/RoleClaims/5
        [HttpGet("getClaimsByRoleId")]
        public async Task<ActionResponse<ApplicationRoleClaim[]>> GetClaimsByRoleId([FromQuery] string roleId)
        {
            return await _roleManager.GetClaimsAsync(new ApplicationRole() { Id = roleId })
                .ContinueWith<ActionResponse<ApplicationRoleClaim[]>>(c =>
                {
                    ActionResponse<ApplicationRoleClaim[]> result = new ActionResponse<ApplicationRoleClaim[]>()
                    {
                        Status = 200
                    };
                    List<ApplicationRoleClaim> list = new List<ApplicationRoleClaim>();
                    if (c.IsCompleted && c.Result.Count > 0)
                    {
                        foreach (var c1 in c.Result)
                        {
                            list.Add(new ApplicationRoleClaim()
                            {
                                Id = IdGenerator.GenerateObjectId().ToString(),
                                RoleId = roleId,
                                ClaimType = c1.Type,
                                ClaimValue = c1.Value
                            });
                        }
                    }

                    result.Data = list.ToArray();
                    return result;
                });
        }

        // PUT: api/RoleClaims/5
        [HttpPost("addRoleClaim")]
        public async Task<ApplicationRoleClaim> AddRoleClaim([FromBody] ApplicationRoleClaim roleClaim)
        {
            var result = await _roleManager.AddClaimAsync(new ApplicationRole() { Id = roleClaim.Id },
                new System.Security.Claims.Claim(roleClaim.ClaimType, roleClaim.ClaimValue));
            if (result.Succeeded)
                return roleClaim;

            return null;
        }

        // POST: api/RoleClaims 
        [HttpPost("removeRoleClaim")]
        public async Task<ApplicationRoleClaim> RemoveRoleClaimAsync([FromBody] ApplicationRoleClaim roleClaim)
        {
            var result = await _roleManager.RemoveClaimAsync(new ApplicationRole() { Id = roleClaim.Id },
                   new System.Security.Claims.Claim(roleClaim.ClaimType, roleClaim.ClaimValue));
            if (!result.Succeeded)
                return roleClaim;

            return null;
        }
    }
}
