using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Celia.io.Core.Auths.Abstractions;
using Celia.io.Core.Auths.Services;
using Celia.io.Core.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Celia.io.Core.Auths.WebAPI_Core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserLoginsController : ControllerBase
    {
        private readonly ILogger<UserLoginsController> _logger;
        private readonly ApplicationUserManager _userManager;

        public UserLoginsController(ILogger<UserLoginsController> logger, ApplicationUserManager userManager)
        {
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        // GET: api/UserLogins
        [HttpPost("adduserlogin")]
        public async Task<ActionResponse<ApplicationUserLogin>> AddUserLogin([FromBody] ApplicationUserLogin userLogin)
        {
            if (userLogin != null && !string.IsNullOrEmpty(userLogin.UserId))
            {
                var user = await _userManager.FindByIdAsync(userLogin.UserId);
                if (user != null)
                {
                    var result = await this._userManager.AddLoginAsync(user,
                         new UserLoginInfo(userLogin.LoginProvider, userLogin.ProviderKey, userLogin.ProviderDisplayName));
                    if (result.Succeeded || (result.Errors != null &&
                        (result.Errors.Count() > 0 && result.Errors.First().Code.Equals("LoginAlreadyAssociated"))))
                        return new ActionResponse<ApplicationUserLogin>() { Status = 200, Data = userLogin };

                    if (!result.Succeeded)
                    {
                        return new ActionResponse<ApplicationUserLogin>()
                        {
                            Status = (int)System.Net.HttpStatusCode.NotAcceptable,
                            ErrorMessage = result.Errors?.FirstOrDefault()?.Description
                        };
                    }
                }
            }

            return new ActionResponse<ApplicationUserLogin>() { Status = 400, ErrorMessage = "Parameter is invalid." };
        }

        // GET: api/UserLogins/5
        [HttpPost("removeuserlogin")]
        public async Task<ActionResponse<string>> RemoveUserLogin([FromBody] ApplicationUserLogin userLogin)
        {
            if (userLogin != null)
            {
                return await this._userManager.RemoveLoginAsync(
                    new ApplicationUser() { Id = userLogin.UserId },
                    userLogin.LoginProvider, userLogin.ProviderKey)
                    .ContinueWith((m) =>
                    {
                        return new ActionResponse<string>() { Status = 200 };
                    });
            }

            return new ActionResponse<string>() { Status = 400, ErrorMessage = "Parameter is invalid." };
        }

        // POST: api/UserLogins
        [HttpGet("getloginsbyuserid")]
        public async Task<ActionResponse<ApplicationUserLogin[]>> GetLoginsByUserId([FromQuery] [Required] string userId)
        {
            var result = await _userManager.GetLoginsAsync(new ApplicationUser() { Id = userId });

            if (result != null && result.Count > 0)
            {
                var list = from one in result
                           select new ApplicationUserLogin()
                           {
                               UserId = userId,
                               LoginProvider = one.LoginProvider,
                               ProviderKey = one.ProviderKey,
                               ProviderDisplayName = one.ProviderDisplayName
                           };

                return new ActionResponse<ApplicationUserLogin[]>() { Status = 200, Data = list.ToArray() };
            }

            return new ActionResponse<ApplicationUserLogin[]> { Status = 200 };
        }

        // PUT: api/UserLogins/5
        [HttpGet("getloginbyuseridlogintype")]
        public async Task<ActionResponse<ApplicationUserLogin>> GetLoginByUserIdLoginType(
            [FromQuery] [Required] string userId, [FromQuery] string loginProvider
            , [FromQuery] string providerKey)
        {
            var result = await _userManager.GetLoginsAsync(new ApplicationUser() { Id = userId });
            if (result != null && result.Count > 0)
            {
                var result2 = result.FirstOrDefault(m =>
                m.LoginProvider.Equals(loginProvider, StringComparison.InvariantCultureIgnoreCase)
                && m.ProviderKey.Equals(providerKey, StringComparison.InvariantCultureIgnoreCase));
                if (result2 != null)
                {
                    return new ActionResponse<ApplicationUserLogin>()
                    {
                        Status = 200,
                        Data = new ApplicationUserLogin()
                        {
                            UserId = userId,
                            LoginProvider = result2.LoginProvider,
                            ProviderKey = result2.ProviderKey,
                            ProviderDisplayName = result2.ProviderDisplayName
                        }
                    };
                }
            }

            return new ActionResponse<ApplicationUserLogin>() { Status = 200 };
        }

        //// DELETE: api/ApiWithActions/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
