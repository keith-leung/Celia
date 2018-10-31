using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Celia.io.Core.Auths.Abstractions;
using Celia.io.Core.Auths.Services;
using Celia.io.Core.Auths.WebAPI_Core.Models; 
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections;
using Celia.io.Core.Utils;

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
        public async Task<ActionResponse<ApplicationRole[]>> List([FromQuery] SimpleSort simpleSort)
        {
            try
            {
                int pageSize2 = 20;
                if (simpleSort.PageSize.HasValue)
                    pageSize2 = simpleSort.PageSize.Value;

                if (simpleSort.OrderType.HasValue && simpleSort.OrderType.Value != 0)
                {
                    return new ActionResponse<ApplicationRole[]>()
                    {
                        Status = 200,
                        Data = (this._roleManager.Roles.OrderByDescending(m => m.NormalizedName)
                        .Skip((simpleSort.PageIndex - 1) * pageSize2).Take(pageSize2)
                        ).ToArray()
                    };
                }

                return new ActionResponse<ApplicationRole[]>()
                {
                    Status = 200,
                    Data = (this._roleManager.Roles.OrderBy(m => m.NormalizedName)
                        .Skip((simpleSort.PageIndex - 1) * pageSize2).Take(pageSize2)
                        ).ToArray()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RolesController.list", simpleSort);
                return new ActionResponse<ApplicationRole[]>()
                {
                    Status = 500,
                    ErrorMessage = ex.Message,
                };
            }
        }

        // GET: api/Roles/findbyid/{id}
        [HttpGet("findbyid")]
        public async Task<ActionResponse<ApplicationRole>> FindById([FromQuery] [Required] string roleId)
        {
            return await this._roleManager.FindByIdAsync(roleId)
                .ContinueWith<ActionResponse<ApplicationRole>>((r) =>
                {
                    if (r.IsCompleted && !r.IsFaulted)
                    {
                        return new ActionResponse<ApplicationRole>() { Status = 200, Data = r.Result };
                    } 
                    return new ActionResponse<ApplicationRole>() { Status = 400, ErrorMessage = r.Exception?.Message };
                });
        }

        [HttpGet("findbyname")]
        public async Task<ActionResponse<ApplicationRole>> FindByName([FromQuery] [Required] string roleName)
        {
            return await this._roleManager.FindByNameAsync(roleName)
                .ContinueWith((r) =>
                {
                    if (r.IsCompleted && !r.IsFaulted)
                    {
                        return new ActionResponse<ApplicationRole>() { Status = 200, Data = r.Result };
                    }
                    return new ActionResponse<ApplicationRole>() { Status = 400, ErrorMessage = r.Exception?.Message };
                });
        }

        [HttpPost("create")]
        public async Task<ActionResponse<ApplicationRole>> Create([FromBody] ApplicationRole role)
        {
            return await this._roleManager.CreateAsync(role)
                .ContinueWith<ActionResponse<ApplicationRole>>((r) =>
                {
                    if (r.IsCompleted && !r.IsFaulted && r.Result.Succeeded)
                    {
                        var result2 = _roleManager.FindByNameAsync(role.Name);
                        result2.Wait();

                        return new ActionResponse<ApplicationRole>()
                        {
                            Status = 200,
                            Data = result2.Result
                        };
                    }
                    else if (r.IsFaulted)
                    {
                        return new ActionResponse<ApplicationRole>()
                        {
                            Status = 400,
                            ErrorMessage = r.Exception?.Message,
                        };
                    }
                    else if (!r.Result.Succeeded)
                    {
                        return new ActionResponse<ApplicationRole>()
                        {
                            Status = (int)System.Net.HttpStatusCode.ExpectationFailed,
                            ErrorMessage = r.Result.Errors?.FirstOrDefault()?.Description
                        };
                    }

                    return new ActionResponse<ApplicationRole>() { Status = 200 };//, Data = r.Result };
                });
        }

        [HttpPost("update")]
        public async Task<ActionResponse<ApplicationRole>> Update([FromBody] ApplicationRole role)
        {
            return await this._roleManager.UpdateAsync(role)
                .ContinueWith<ActionResponse<ApplicationRole>>((r) =>
                {
                    if (r.IsCompleted && !r.IsFaulted && r.Result.Succeeded)
                    {
                        return new ActionResponse<ApplicationRole>()
                        {
                            Status = 200,
                        };
                    }
                    else if (r.IsFaulted)
                    {
                        return new ActionResponse<ApplicationRole>()
                        {
                            Status = 400,
                            ErrorMessage = r.Exception?.Message,
                        };
                    }
                    else if (!r.Result.Succeeded)
                    {
                        return new ActionResponse<ApplicationRole>()
                        {
                            Status = (int)System.Net.HttpStatusCode.ExpectationFailed,
                            ErrorMessage = r.Result.Errors?.FirstOrDefault()?.Description
                        };
                    }

                    return new ActionResponse<ApplicationRole>() { Status = 200 };
                });
        }

        [HttpPost("delete")]
        public async Task<ActionResponse<ApplicationRole>> Delete([FromBody] ApplicationRole role)
        {
            return await this._roleManager.DeleteAsync(role)
                .ContinueWith<ActionResponse<ApplicationRole>>((r) =>
                {
                    if (r.IsCompleted && !r.IsFaulted && r.Result.Succeeded)
                    {
                        return new ActionResponse<ApplicationRole>()
                        {
                            Status = 200,
                        };
                    }
                    else if (r.IsFaulted)
                    {
                        return new ActionResponse<ApplicationRole>()
                        {
                            Status = 400,
                            ErrorMessage = r.Exception?.Message,
                        };
                    }
                    else if (!r.Result.Succeeded)
                    {
                        return new ActionResponse<ApplicationRole>()
                        {
                            Status = (int)System.Net.HttpStatusCode.ExpectationFailed,
                            ErrorMessage = r.Result.Errors?.FirstOrDefault()?.Description
                        };
                    }

                    return new ActionResponse<ApplicationRole>() { Status = 200 };
                });
        }

        [HttpPost("addclaimstorole")]
        public async Task<ActionResponse<ApplicationRoleClaim[]>> AddClaimsToRole(
            [FromBody] IEnumerable<ApplicationRoleClaim> roleClaims)
        {
            foreach (var rc in roleClaims)
            {
                var res1 = await _roleManager.AddClaimAsync(
                    new ApplicationRole()
                    {
                        Id = rc.RoleId
                    },
                    new System.Security.Claims.Claim(
                        rc.ClaimType,
                        rc.ClaimValue));

                if (!res1.Succeeded)
                {
                    return new ActionResponse<ApplicationRoleClaim[]>()
                    {
                        Status = 400,
                        ErrorMessage = res1.Errors?.FirstOrDefault()?.Description
                    };
                }
            }

            return new ActionResponse<ApplicationRoleClaim[]>()
            {
                Status = 400,
                Data = roleClaims.ToArray()
            };
        }

        [HttpGet("getclaimsbyroleid")]
        public async Task<ActionResponse<ApplicationRoleClaim[]>> GetClaimsByRoleId(
            [FromQuery] [Required] string roleId)
        {
            ApplicationRole role = await _roleManager.FindByIdAsync(roleId);
            if (role != null)
            {
                var result = await _roleManager.GetClaimsAsync(role);
                return new ActionResponse<ApplicationRoleClaim[]>()
                {
                    Status = 200,
                    Data = (from one in result
                            select new ApplicationRoleClaim()
                            {
                                RoleId = roleId,
                                ClaimType = one.Type,
                                ClaimValue = one.Value
                            }).ToArray()
                };
            }

            return new ActionResponse<ApplicationRoleClaim[]>()
            {
                Status = 403,
                ErrorMessage = "Role does not exists"
            };
        }
    }
}
