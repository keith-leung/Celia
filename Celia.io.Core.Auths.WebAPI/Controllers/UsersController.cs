using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Celia.io.Core.Auths.Abstractions;
using Celia.io.Core.Auths.Abstractions.DTOs;
using Celia.io.Core.Auths.Services;
using Celia.io.Core.Auths.WebAPI_Core.Models; 
using Celia.io.Core.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Celia.io.Core.Auths.WebAPI_Core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;
        private readonly ApplicationUserManager _userManager;
        private readonly ApplicationRoleManager _roleManager;

        public UsersController(ILogger<UsersController> logger, ApplicationUserManager userManager,
            ApplicationRoleManager roleManager)
        {
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            this._roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
        }

        // GET: api/Users/list
        [HttpGet("list")]
        public async Task<ActionResponse<ApplicationUser[]>> List([FromQuery] UserSort userSort)
        {
            try
            {
                int pageSize2 = 20;
                if (userSort.PageSize.HasValue)
                    pageSize2 = userSort.PageSize.Value;

                IQueryable<ApplicationUser> users = this._userManager.Users;

                if (userSort != null && !string.IsNullOrEmpty(userSort.SortBy)
                    && userSort.SortBy.Equals(UserSort.SORT_BY_EMAIL, StringComparison.InvariantCultureIgnoreCase))
                {
                    if (userSort != null && (userSort.OrderType.HasValue && userSort.OrderType.Value != 0))
                    {
                        users = users.OrderByDescending(m => m.Email);
                    }
                    else
                    {
                        users = users.OrderBy(m => m.Email);
                    }
                }
                else
                {
                    if (userSort != null && (userSort.OrderType.HasValue && userSort.OrderType.Value != 0))
                    {
                        users = users.OrderByDescending(m => m.UserName);
                    }
                    else
                    {
                        users = users.OrderBy(m => m.UserName);
                    }
                }

                return new ActionResponse<ApplicationUser[]>()
                {
                    Status = 200,
                    Data = users.Skip((userSort.PageIndex - 1) * pageSize2).Take(pageSize2)
                     .ToArray()
                };
            }
            catch (Exception ex)
            {
                return new ActionResponse<ApplicationUser[]>()
                {
                    Status = 500,
                    ErrorMessage = ex.Message,
                };
            }
        }

        [HttpGet("listbyusername")]
        public async Task<ActionResponse<ApplicationUser[]>> ListByUserName([FromQuery] UserSort userSort,
            [FromQuery] [Required] string username)
        {
            try
            {
                int pageSize2 = 20;
                if (userSort.PageSize.HasValue)
                    pageSize2 = userSort.PageSize.Value;

                IQueryable<ApplicationUser> users = this._userManager.Users.Where(
                    m => m.UserName.Contains(username, StringComparison.InvariantCultureIgnoreCase));

                if (userSort != null && !string.IsNullOrEmpty(userSort.SortBy)
                    && userSort.SortBy.Equals(UserSort.SORT_BY_EMAIL, StringComparison.InvariantCultureIgnoreCase))
                {
                    if (userSort != null && (userSort.OrderType.HasValue && userSort.OrderType.Value != 0))
                    {
                        users = users.OrderByDescending(m => m.Email);
                    }
                    else
                    {
                        users = users.OrderBy(m => m.Email);
                    }
                }
                else
                {
                    if (userSort != null && (userSort.OrderType.HasValue && userSort.OrderType.Value != 0))
                    {
                        users = users.OrderByDescending(m => m.UserName);
                    }
                    else
                    {
                        users = users.OrderBy(m => m.UserName);
                    }
                }

                return new ActionResponse<ApplicationUser[]>()
                {
                    Status = 200,
                    Data = users.Skip((userSort.PageIndex - 1) * pageSize2).Take(pageSize2)
                     .ToArray()
                };
            }
            catch (Exception ex)
            {
                return new ActionResponse<ApplicationUser[]>()
                {
                    Status = 500,
                    ErrorMessage = ex.Message,
                };
            }
        }

        [HttpGet("listbyphonenumber")]
        public async Task<ActionResponse<ApplicationUser[]>> ListByPhoneNumber([FromQuery] UserSort userSort,
            [FromQuery] [Required] string phoneNumber)
        {
            try
            {
                int pageSize2 = 20;
                if (userSort.PageSize.HasValue)
                    pageSize2 = userSort.PageSize.Value;

                IQueryable<ApplicationUser> users = this._userManager.Users.Where(
                    m => !string.IsNullOrEmpty(m.PhoneNumber)
                    && m.PhoneNumber.Contains(phoneNumber, StringComparison.InvariantCultureIgnoreCase));

                if (userSort != null && !string.IsNullOrEmpty(userSort.SortBy)
                    && userSort.SortBy.Equals(UserSort.SORT_BY_EMAIL, StringComparison.InvariantCultureIgnoreCase))
                {
                    if (userSort != null && (userSort.OrderType.HasValue && userSort.OrderType.Value != 0))
                    {
                        users = users.OrderByDescending(m => m.Email);
                    }
                    else
                    {
                        users = users.OrderBy(m => m.Email);
                    }
                }
                else
                {
                    if (userSort != null && (userSort.OrderType.HasValue && userSort.OrderType.Value != 0))
                    {
                        users = users.OrderByDescending(m => m.UserName);
                    }
                    else
                    {
                        users = users.OrderBy(m => m.UserName);
                    }
                }

                return new ActionResponse<ApplicationUser[]>()
                {
                    Status = 200,
                    Data = users.Skip((userSort.PageIndex - 1) * pageSize2).Take(pageSize2)
                     .ToArray()
                };
            }
            catch (Exception ex)
            {
                return new ActionResponse<ApplicationUser[]>()
                {
                    Status = 500,
                    ErrorMessage = ex.Message,
                };
            }
        }

        [HttpGet("findbyid")]
        public async Task<ActionResponse<ApplicationUser>> FindByUserId([FromQuery] [Required] string userid)
        {
            try
            {
                return await _userManager.FindByIdAsync(userid)
                    .ContinueWith<ActionResponse<ApplicationUser>>((a) =>
                    {
                        if (a.IsCompletedSuccessfully && a.Result != null)
                        {
                            return new ActionResponse<ApplicationUser>()
                            {
                                Status = 200,
                                Data = a.Result
                            };
                        }
                        else if (a.IsCompleted && a.IsFaulted)
                        {
                            return new ActionResponse<ApplicationUser>()
                            {
                                Status = 500,
                                ErrorMessage = a.Exception?.InnerException?.Message
                            };
                        }

                        return new ActionResponse<ApplicationUser>()
                        {
                            Status = 200,
                        };
                    });
            }
            catch (Exception ex)
            {
                return new ActionResponse<ApplicationUser>()
                {
                    Status = 500,
                    ErrorMessage = ex.Message,
                };
            }
        }

        [HttpGet("finduserbylogin")]
        public async Task<ActionResponse<ApplicationUser>> FindUserByLogin([FromQuery] [Required] string loginProvider,
            [FromQuery] [Required] string providerKey)
        {
            try
            {
                return await _userManager.FindByLoginAsync(loginProvider, providerKey)
                .ContinueWith((a) =>
                {
                    if (a.IsCompletedSuccessfully && a.Result != null)
                    {
                        return new ActionResponse<ApplicationUser>()
                        {
                            Status = 200,
                            Data = a.Result
                        };
                    }
                    else if (a.IsCompleted && a.IsFaulted)
                    {
                        return new ActionResponse<ApplicationUser>()
                        {
                            Status = 500,
                            ErrorMessage = a.Exception?.InnerException?.Message
                        };
                    }
                    else
                    {
                        return new ActionResponse<ApplicationUser>()
                        {
                            Status = 200,
                        };
                    }
                });
            }
            catch (Exception ex)
            {
                return new ActionResponse<ApplicationUser>()
                {
                    Status = 500,
                    ErrorMessage = ex.Message,
                };
            }
        }

        [HttpGet("findbyname")]
        public async Task<ActionResponse<ApplicationUser>> FindByUserName([FromQuery] [Required] string name)
        {
            try
            {
                return await _userManager.FindByNameAsync(name)
                .ContinueWith((a) =>
                {
                    if (a.IsCompletedSuccessfully && a.Result != null)
                    {
                        return new ActionResponse<ApplicationUser>()
                        {
                            Status = 200,
                            Data = a.Result
                        };
                    }
                    else if (a.IsCompleted && a.IsFaulted)
                    {
                        return new ActionResponse<ApplicationUser>()
                        {
                            Status = 500,
                            ErrorMessage = a.Exception?.InnerException?.Message
                        };
                    }
                    else
                    {
                        return new ActionResponse<ApplicationUser>()
                        {
                            Status = 200,
                        };
                    }
                });
            }
            catch (Exception ex)
            {
                return new ActionResponse<ApplicationUser>()
                {
                    Status = 500,
                    ErrorMessage = ex.Message,
                };
            }
        }

        [HttpGet("findbyemail")]
        public async Task<ActionResponse<ApplicationUser>> FindByEmail([FromQuery] [Required] string email)
        {
            try
            {
                return await _userManager.FindByEmailAsync(email)
                .ContinueWith((a) =>
                {
                    if (a.IsCompletedSuccessfully && a.Result != null)
                    {
                        return new ActionResponse<ApplicationUser>()
                        {
                            Status = 200,
                            Data = a.Result
                        };
                    }
                    else if (a.IsCompleted && a.IsFaulted)
                    {
                        return new ActionResponse<ApplicationUser>()
                        {
                            Status = 500,
                            ErrorMessage = a.Exception?.InnerException?.Message
                        };
                    }
                    else
                    {
                        return new ActionResponse<ApplicationUser>()
                        {
                            Status = 200,
                        };
                    }
                });
            }
            catch (Exception ex)
            {
                return new ActionResponse<ApplicationUser>()
                {
                    Status = 500,
                    ErrorMessage = ex.Message,
                };
            }
        }

        // GET api/values
        [HttpPost("adduser")]
        public async Task<ActionResponse<ApplicationUser>> AddUser([FromBody] ApplicationUser user)
        {
            try
            {
                var result = await _userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    return new ActionResponse<ApplicationUser>()
                    {
                        Status = 200,
                        Data = user,
                    };
                }
                else if (result.Errors != null && result.Errors.Count() > 0)
                {
                    var error = result.Errors.First();
                    if (error.Code.Equals("DuplicateUserName"))
                    {
                        return new ActionResponse<ApplicationUser>()
                        {
                            Status = 403001,
                            ErrorMessage = error.Description,
                        };
                    }
                }
                return new ActionResponse<ApplicationUser>()
                {
                    Status = (int)System.Net.HttpStatusCode.BadRequest,
                    ErrorMessage = "请求信息不正确，无法添加用户。"
                };
            }
            catch (Exception ex)
            {
                return new ActionResponse<ApplicationUser>()
                {
                    Status = 500,
                    ErrorMessage = ex.Message,
                };
            }
        }

        [HttpPost("changepassword")]
        public async Task<ActionResponse<string>> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(request.UserId);
                var result = await _userManager.ChangePasswordAsync(user, request.OldPassword, request.NewPassword);

                if (result.Succeeded)
                {
                    return new ActionResponse<string>()
                    {
                        Status = 200,
                    };
                }

                string msg = result.Errors?.FirstOrDefault()?.Description;

                return new ActionResponse<string>()
                {
                    Status = 400,
                    ErrorMessage = msg,
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

        [HttpPost("resetpassword")]
        public async Task<ActionResponse<string>> ResetUserPassword([FromBody] KVRequest request)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(request.Key);
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, token, request.Value);

                if (result.Succeeded)
                {
                    return new ActionResponse<string>()
                    {
                        Status = 200,
                    };
                }

                string msg = result.Errors?.FirstOrDefault()?.Description;

                return new ActionResponse<string>()
                {
                    Status = 400,
                    ErrorMessage = msg,
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

        // GET api/values
        [HttpPost("updateuser")]
        public async Task<ActionResponse<ApplicationUser>> UpdateUser([FromBody] ApplicationUser user)
        {
            try
            {
                return await _userManager.FindByIdAsync(user.Id)
                //return await _userManager.UpdateAsync(user)
                    .ContinueWith((result) =>
                    {
                        var res = new ActionResponse<ApplicationUser>()
                        {
                            Status = 400,
                        };

                        if (result.IsCompletedSuccessfully && result.Result != null)
                        {
                            var entity = result.Result;
                            entity.UserName = user.UserName;
                            entity.PhoneNumber = user.PhoneNumber;
                            entity.PhoneNumberConfirmed = user.PhoneNumberConfirmed;
                            entity.TwoFactorEnabled = user.TwoFactorEnabled;
                            entity.LockoutEnd = user.LockoutEnd;
                            entity.LockoutEnabled = user.LockoutEnabled;
                            entity.EmailConfirmed = user.EmailConfirmed;
                            entity.Email = user.Email;
                            entity.AccessFailedCount = user.AccessFailedCount;

                            var temp = _userManager.UpdateAsync(entity);
                            temp.Wait();

                            if (temp.IsCompletedSuccessfully && temp.Result.Succeeded)
                            {
                                return new ActionResponse<ApplicationUser>()
                                {
                                    Data = entity,
                                    Status = 200,
                                };
                            }

                            res.ErrorMessage = temp?.Result?.Errors?.FirstOrDefault()?.Description;
                        }

                        //if (result.Result.Errors.Count() > 0)
                        //{
                        //    res.ErrorMessage =result.Result.Errors.First().Description;
                        //}

                        return res;
                    });
            }
            catch (Exception ex)
            {
                return new ActionResponse<ApplicationUser>()
                {
                    Status = 500,
                    ErrorMessage = ex.Message,
                };
            }
        }

        // GET api/values
        [HttpPost("deleteuser")]
        public async Task<ActionResponse<string>> DeleteUser([FromBody] ApplicationUser user)
        {
            try
            {
                var entity = await _userManager.FindByIdAsync(user.Id);
                if (entity != null)
                {
                    var loginList = await _userManager.GetLoginsAsync(entity);

                    var totalResult = new ActionResponse<string>();

                    if (loginList != null && loginList.Count() > 0)
                    {
                        Parallel.ForEach(loginList, async (login) =>
                        {
                            var removeLoginResult =
                                await _userManager.RemoveLoginAsync(user, login.LoginProvider, login.ProviderKey);
                            if (!removeLoginResult.Succeeded)
                            {
                                totalResult.Status = 400;
                                totalResult.ErrorMessage = removeLoginResult?.Errors?.FirstOrDefault()?.Description;
                            }
                        });

                        if (totalResult.Status == 400)
                            return totalResult;
                    }

                    var roleList = await _userManager.GetRolesAsync(entity);
                    var identity1 = await _userManager.RemoveFromRolesAsync(entity, roleList);
                    if (!identity1.Succeeded)
                    {
                        return new ActionResponse<string>()
                        {
                            Status = 400,
                            ErrorMessage = identity1.Errors?.FirstOrDefault()?.Description
                        };
                    }

                    var claimList = await _userManager.GetClaimsAsync(entity);
                    var identity2 = await _userManager.RemoveClaimsAsync(entity, claimList);
                    if (!identity2.Succeeded)
                    {
                        return new ActionResponse<string>()
                        {
                            Status = 400,
                            ErrorMessage = identity1.Errors?.FirstOrDefault()?.Description
                        };
                    }

                    var result = await _userManager.DeleteAsync(entity);
                    if (result.Succeeded)
                    {
                        return new ActionResponse<string>()
                        {
                            Status = 200
                        };
                    }

                    string msg = result.Errors?.FirstOrDefault()?.Description;

                    return new ActionResponse<string>()
                    {
                        Status = 400,
                        ErrorMessage = msg,
                    };
                }

                return new ActionResponse<string>()
                {
                    Status = 403,
                    ErrorMessage = "User does not exist. ",
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

        [HttpGet("getrolesbyuserid")]
        public async Task<ActionResponse<ApplicationRole[]>> GetRolesByUserId([FromQuery] [Required] string userId)
        {
            try
            {
                var result = await _userManager.GetRolesAsync(new ApplicationUser() { Id = userId });
                if (result != null && result.Count > 0)
                {
                    return new ActionResponse<ApplicationRole[]>()
                    {
                        Status = 200,
                        Data = (from one in result
                                select new ApplicationRole() { Name = one })
                                .ToArray(),
                    };
                }

                return new ActionResponse<ApplicationRole[]>() { Status = 200, Data = new ApplicationRole[] { } };
            }
            catch (Exception ex)
            {
                return new ActionResponse<ApplicationRole[]>()
                {
                    Status = 500,
                    ErrorMessage = ex.Message,
                };
            }
        }

        [HttpGet("getclaimsbyuserid")]
        public async Task<ActionResponse<ApplicationUserClaim[]>> GetClaimsByRoleId([FromQuery] [Required] string userId)
        {
            try
            {
                var result = await _userManager.GetClaimsAsync(new ApplicationUser() { Id = userId });
                if (result != null)
                {
                    return new ActionResponse<ApplicationUserClaim[]>()
                    {
                        Status = 200,
                        Data =
                        (from one in result
                         select new ApplicationUserClaim()
                         {
                             UserId = userId,
                             ClaimType = one.Type,
                             ClaimValue = one.Value
                         })
                            .ToArray(),
                    };
                }

                return new ActionResponse<ApplicationUserClaim[]>()
                {
                    Status = 200,
                };
            }
            catch (Exception ex)
            {
                return new ActionResponse<ApplicationUserClaim[]>()
                {
                    Status = 500,
                    ErrorMessage = ex.Message,
                };
            }
        }

        [HttpPost("lockuser")]
        public async Task<ActionResponse<string>> LockUser([FromBody] LockoutUserRequest request)
        {
            return await _userManager.FindByIdAsync(request.UserId)
                .ContinueWith(
                        (userResult) =>
                        {
                            if (userResult.IsCompletedSuccessfully && userResult.Result != null)
                            {
                                var res1 = _userManager.SetLockoutEnabledAsync(userResult.Result, true);
                                res1.Wait();
                                DateTimeOffset offset = new DateTimeOffset(request.LockoutEndTimeUtc);
                                //request.LockoutEndTimeUtc.Subtract(DateTime.UtcNow)
                                var res2 = _userManager.SetLockoutEndDateAsync(userResult.Result, offset);
                                res2.Wait();
                                return new ActionResponse<string>()
                                {
                                    Status = 200,
                                };
                            };

                            return new ActionResponse<string>()
                            {
                                Status = 403,
                                ErrorMessage = "User does not exist.",
                            };
                        });
        }

        [HttpPost("unlockuser")]
        public async Task<ActionResponse<string>> UnlockUser([FromBody] ApplicationUser user)
        {
            return await _userManager.FindByIdAsync(user.Id)
                .ContinueWith(
                        (userResult) =>
                        {
                            if (userResult.IsCompletedSuccessfully && userResult.Result != null)
                            {
                                var res1 = _userManager.SetLockoutEnabledAsync(userResult.Result, false);
                                res1.Wait();

                                var res2 = _userManager.SetLockoutEndDateAsync(userResult.Result, null);
                                res2.Wait();
                                return new ActionResponse<string>()
                                {
                                    Status = 200,
                                };
                            };

                            return new ActionResponse<string>()
                            {
                                Status = 403,
                                ErrorMessage = "User does not exist.",
                            };
                        });
        }
    }
}