using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Celia.io.Core.Auths.Abstractions;
using Celia.io.Core.Auths.Services;
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
        public async Task<ApplicationUserLogin> AddUserLogin([FromBody] ApplicationUserLogin userLogin)
        {
            if (userLogin != null)
            {
                var result = await this._userManager.AddLoginAsync(new ApplicationUser() { Id = userLogin.UserId },
                     new UserLoginInfo(userLogin.LoginProvider, userLogin.ProviderKey, userLogin.ProviderDisplayName));
                if (result.Succeeded)
                    return userLogin;
            }
            return null;
        }

        // GET: api/UserLogins/5
        [HttpPost("removeuserlogin")]
        public async Task RemoveUserLogin([FromBody] ApplicationUserLogin userLogin)
        {
            if (userLogin != null)
            {
                var result = await this._userManager.RemoveLoginAsync(
                    new ApplicationUser() { Id = userLogin.UserId },
                    userLogin.LoginProvider, userLogin.ProviderKey);
            }
        }

        // POST: api/UserLogins
        [HttpGet("getloginsbyuserid")]
        public async Task<ApplicationUserLogin[]> GetLoginsByUserId([FromQuery] [Required] string userId)
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

                return list.ToArray();
            }
            return new ApplicationUserLogin[] { };
        }

        // PUT: api/UserLogins/5
        [HttpGet("getloginbyuseridlogintype")]
        public async Task<ApplicationUserLogin> GetLoginByUserIdLoginType(
            [FromQuery] [Required] string userId, [FromQuery] string loginProvider)
        {
            var result = await _userManager.GetLoginsAsync(new ApplicationUser() { Id = userId });
            if (result != null && result.Count > 0)
            {
                var result2 = result.FirstOrDefault(m =>
                m.LoginProvider.Equals(loginProvider, StringComparison.InvariantCultureIgnoreCase));
                if (result2 != null)
                {
                    return new ApplicationUserLogin()
                    {
                        UserId = userId,
                        LoginProvider = result2.LoginProvider,
                        ProviderKey = result2.ProviderKey,
                        ProviderDisplayName = result2.ProviderDisplayName
                    };
                }
            }
            return null;
        }

        //// DELETE: api/ApiWithActions/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
