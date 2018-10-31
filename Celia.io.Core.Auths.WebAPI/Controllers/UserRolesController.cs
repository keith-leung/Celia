using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Celia.io.Core.Auths.Abstractions;
using Celia.io.Core.Auths.Services;
using Celia.io.Core.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Celia.io.Core.Auths.WebAPI_Core.Controllers
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = "Bearer")]
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
        [AllowAnonymous]
        public async Task<ActionResponse<ApplicationUserRole[]>> AddUserRoles(
            [FromBody] IEnumerable<ApplicationUserRole> userRoles)
        {
            if (userRoles == null || userRoles.Count() < 1)
            {
                return new ActionResponse<ApplicationUserRole[]>()
                {
                    Status = 200,
                    Data = new ApplicationUserRole[] { }
                };
            }

            return await _userManager.FindByIdAsync(userRoles.First().UserId)
                .ContinueWith<ActionResponse<ApplicationUserRole[]>>((m) =>
                {
                    if (m.IsCompleted && !m.IsFaulted && m.Result != null)
                    {
                        IEnumerable<string> roles = from one in userRoles
                                                    select one.RoleId;

                        var roleNames = _roleManager.Roles.Where(m1 => roles.Contains(m1.Id))
                            .Select(m2 => m2.Name);

                        var result = _userManager.AddToRolesAsync(m.Result, roleNames);
                        result.Wait();
                        if (result.IsCompleted && !result.IsFaulted && result.Result.Succeeded)
                        {
                            return new ActionResponse<ApplicationUserRole[]>() { Status = 200, Data = userRoles.ToArray() };
                        }
                    }

                    return new ActionResponse<ApplicationUserRole[]>()
                    {
                        Status = 200,
                        Data = new ApplicationUserRole[] { }
                    };
                });
        }

        // POST: api/UserRoles
        [HttpPost("adduserrole")]
        [AllowAnonymous]
        public async Task<ActionResponse<ApplicationUserRole>> AddUserRole(
            [FromBody] ApplicationUserRole userRole)
        {
            try
            {
                if (userRole != null && !string.IsNullOrEmpty(userRole.UserId)
                    && !string.IsNullOrEmpty(userRole.RoleId))
                {
                    ApplicationUser user = await _userManager.FindByIdAsync(userRole.UserId);
                    if (user != null)
                    {
                        if (await _userManager.IsInRoleAsync(user, userRole.RoleId))
                        {
                            return new ActionResponse<ApplicationUserRole>()
                            {
                                Status = 200,
                                Data = userRole
                            };
                        }

                        var role = await this._roleManager.FindByIdAsync(userRole.RoleId);
                        if (role != null)
                        {
                            var result = await _userManager.AddToRoleAsync(user, role.Name);

                            if (result.Succeeded || result.Errors?.FirstOrDefault()?.Code == "UserAlreadyInRole")
                            {
                                return new ActionResponse<ApplicationUserRole>()
                                {
                                    Status = 200,
                                    Data = userRole
                                };
                            }
                            else
                            {
                                return new ActionResponse<ApplicationUserRole>()
                                {
                                    Status = 403,
                                    ErrorMessage = result.Errors.FirstOrDefault()?.Description,
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UserRolesController.adduserrole", userRole);
            }

            return new ActionResponse<ApplicationUserRole>()
            {
                Status = 400,
                ErrorMessage = "Role does not exist. ",
            };
        }

        // DELETE: api/ApiWithActions/5
        [HttpPost("removeuserrole")]
        public async Task<ActionResponse<string>> RemoveUserRole([FromBody] ApplicationUserRole userRole)
        {
            try
            {
                if (userRole == null || string.IsNullOrEmpty(userRole.UserId))
                    return new ActionResponse<string>() { Status = 200 };

                ApplicationUser user = await _userManager.FindByIdAsync(userRole.UserId);

                if (user == null)
                {
                    return new ActionResponse<string>()
                    {
                        Status = 400,
                        ErrorMessage = "User does not exist. ",
                    };
                }

                var role = await this._roleManager.FindByIdAsync(userRole.RoleId);
                if (role != null)
                {
                    var result = await _userManager.RemoveFromRoleAsync(user, role.Name)
                        .ContinueWith((r) =>
                        {
                            if (!r.IsFaulted && r.Result.Succeeded)
                            {
                                return new ActionResponse<string>()
                                {
                                    Status = 200,
                                };
                            }

                            return new ActionResponse<string>()
                            {
                                Status = 403,
                                ErrorMessage = r.Result?.Errors?.FirstOrDefault()?.Description,// r.Exception?.Message,
                            };
                        });

                    return result;
                }

                return new ActionResponse<string>()
                {
                    Status = 400,
                    ErrorMessage = "Role does not exist. ",
                };
            }
            catch (Exception ex)
            {
                return new ActionResponse<string>()
                {
                    Status = 500,
                    ErrorMessage = ex.Message,
                };
            }
        }

        // GET: api/UserRoles
        [HttpPost("removeuserroles")]
        public async Task<ActionResponse<string>> RemoveUserRoles(
            [FromBody] IEnumerable<ApplicationUserRole> userRoles)
        {
            try
            {
                if (userRoles == null || userRoles.Count() < 1)
                    return new ActionResponse<string>() { Status = 200 };

                ApplicationUser user = await _userManager.FindByIdAsync(userRoles.First().UserId);

                if (user == null)
                {
                    return new ActionResponse<string>()
                    {
                        Status = 400,
                        ErrorMessage = "User does not exist. ",
                    };
                }

                IEnumerable<string> roles = from one in userRoles
                                            select one.RoleId;
                IEnumerable<string> roleNames = _roleManager.Roles.Where(m1 => roles.Contains(m1.Id))
                    .Select(m2 => m2.Name);

                return await _userManager.RemoveFromRolesAsync(user, roleNames)
                    .ContinueWith((r) =>
                    {
                        if (!r.IsFaulted && r.Result.Succeeded)
                        {
                            return new ActionResponse<string>()
                            {
                                Status = 200,
                            };
                        }

                        return new ActionResponse<string>()
                        {
                            Status = 403,
                            ErrorMessage = r.Result?.Errors?.FirstOrDefault()?.Description,// r.Exception?.Message,
                        };
                    });
            }
            catch (Exception ex)
            {
                return new ActionResponse<string>()
                {
                    Status = 500,
                    ErrorMessage = ex.Message,
                };
            }
        }

        [HttpPost("removeuserallroles")]
        public async Task<ActionResponse<string>> RemoveUserAllRoles(
            [FromBody] ApplicationUser user)
        {
            if (user != null && !string.IsNullOrEmpty(user.Id))
            {
                user = await _userManager.FindByIdAsync(user.Id);
                if (user == null)
                {
                    return new ActionResponse<string>()
                    {
                        Status = 400,
                        ErrorMessage = "User does not exist. ",
                    };
                }

                return await _userManager.GetRolesAsync(user)
                    .ContinueWith<ActionResponse<string>>((m) =>
                    {
                        if (m.IsCompleted && !m.IsFaulted && m.Result != null && m.Result.Count > 0)
                        {
                            var task = _userManager.RemoveFromRolesAsync(user, m.Result);
                            task.Wait();

                            if (!task.IsFaulted)
                            {
                                return new ActionResponse<string>()
                                {
                                    Status = 200,
                                };
                            }

                            return new ActionResponse<string>()
                            {
                                Status = 403,
                                ErrorMessage = task.Result?.Errors?.FirstOrDefault()?.Description,
                            };
                        }
                        else
                        {
                            if (!m.IsFaulted)
                            {
                                return new ActionResponse<string>()
                                {
                                    Status = 200,
                                };
                            }

                            return new ActionResponse<string>()
                            {
                                Status = 403,
                                ErrorMessage = m.Exception?.Message,
                            };
                        }
                    });
            }

            return new ActionResponse<string>() { Status = 200 };
        }

        [HttpGet("isinrole")]
        [AllowAnonymous]
        public async Task<bool> IsInRole([FromQuery] [Required] string userId,
            [FromQuery] [Required] string roleName)
        {
            return await _userManager.IsInRoleAsync(new ApplicationUser() { Id = userId }, roleName);
        }
    }
}
