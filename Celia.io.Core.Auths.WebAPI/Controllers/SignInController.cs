using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Celia.io.Core.Auths.Abstractions;
using Celia.io.Core.Auths.Services;
using Celia.io.Core.Auths.WebAPI_Core.Models;
using Celia.io.Core.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Celia.io.Core.Auths.WebAPI_Core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SignInController : ControllerBase
    {
        private readonly ILogger<SignInController> _logger;
        private readonly ApplicationUserManager _userManager;
        private readonly ApplicationSignInManager _signInManager;
        private readonly DisconfService _disconfService;
        private readonly SigningCredentials _signingCredentials;

        private readonly IServiceProvider _serviceProvider;

        public string Issuer { get; set; }
        public string Audience { get; set; }

        public SignInController(IServiceProvider serviceProvider,
            ILogger<SignInController> logger, ApplicationUserManager userManager,
            ApplicationSignInManager signInManager)//, SigningCredentials signingCredentials)
        {
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            this._signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));

            this._serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _signingCredentials = serviceProvider.GetService(typeof(SigningCredentials)) as SigningCredentials;
            _disconfService = serviceProvider.GetService(typeof(DisconfService)) as DisconfService;
        }

        [HttpGet("accesstoken")]
        public async Task<string> AccessToken()
        {
            try
            {
                DateTimeOffset offset = new DateTimeOffset(DateTime.Now);
                offset = offset.AddDays(7); //7天内过期 

                return _signInManager.GetToken(Issuer, Audience, _signingCredentials,
                    HttpContext.User.Identity.Name, HttpContext.User.Claims, offset);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet("refreshtoken")]
        public async Task<string> RefreshToken([FromQuery] [Required] string token)
        {
            try
            {
                DateTimeOffset offset = new DateTimeOffset(DateTime.Now);
                offset = offset.AddDays(7); //7天内过期 

                var result = _signInManager.RefreshToken(token, Issuer, Audience, _signingCredentials,
                          HttpContext.User.Identity.Name, HttpContext.User.Claims, offset);

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet("validatetoken")]
        public async Task<TokenResponse> ValidateToken([FromQuery] [Required] string token)
        {
            try
            {
                return new TokenResponse()
                {
                    IsValid = true,
                    IsRefreshRequired = (false == _signInManager.ValidateToken(token))
                };
            }
            catch (Exception ex)
            {
                return new TokenResponse() { IsValid = false, IsRefreshRequired = true };
            }
        }

        [HttpPost("loginbyusername")]
        [AllowAnonymous]
        public async Task<LoginResponse> LoginByUserName([FromBody] LoginRequest request)
        {
            var username = request.Key;
            string password = request.Value;
            var user = await _userManager.FindByNameAsync(username);
            if (user != null)
            {
                var signInResult = await _signInManager.PasswordSignInAsync(user, password,
                    false, false);
                if (signInResult.Succeeded)
                {
                    string token = await this.GetToken(user);

                    return new LoginResponse()
                    {
                        StatusCode = 200,
                        AccessToken = token,
                        User = user
                    };
                }
            }

            return new LoginResponse() { StatusCode = 401 };
        }

        private async Task<string> GetToken(ApplicationUser user)
        {
            var claimPrincipals = _signInManager.ClaimsFactory.CreateAsync(user);

            try
            {
                DateTimeOffset offset = new DateTimeOffset(DateTime.Now);
                offset = offset.AddDays(7); //7天内过期 

                string token = _signInManager.GetToken(_disconfService.CustomConfigs["Issuer"].ToString(),
                    _disconfService.CustomConfigs["Audience"].ToString(),
                    _signingCredentials,
                    user.Id, await _userManager.GetClaimsAsync(user), //创建用户跟权限的Claim
                    offset);

                return token;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost("loginbyuserid")]
        [AllowAnonymous]
        public async Task<LoginResponse> LoginByUserId([FromBody] LoginRequest request)
        {
            var userid = request.Key;
            string password = request.Value;

            var user = await _userManager.FindByIdAsync(userid);
            if (user != null)
            {
                var signInResult = await _signInManager.PasswordSignInAsync(user.UserName, password,
                    false, false);
                if (signInResult.Succeeded)
                {
                    string token = await this.GetToken(user);

                    return new LoginResponse()
                    {
                        StatusCode = 200,
                        AccessToken = token,
                        User = user
                    };
                }
            }

            return new LoginResponse() { StatusCode = 401 };
        }

        [HttpPost("loginbyemail")]
        [AllowAnonymous]
        public async Task<LoginResponse> LoginByEmail([FromBody] LoginRequest request)
        {
            var email = request.Key;
            string password = request.Value;

            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var signInResult = await _signInManager.PasswordSignInAsync(user, password,
                    false, false);
                if (signInResult.Succeeded)
                {
                    string token = await this.GetToken(user);

                    return new LoginResponse()
                    {
                        StatusCode = 200,
                        AccessToken = token,
                        User = user
                    };
                }
            }

            return new LoginResponse() { StatusCode = 401 };
        }

        [HttpPost("externallogin")]
        [AllowAnonymous]
        public async Task<LoginResponse> ExternalLogin([FromBody] LoginRequest request)
        {
            var loginProvider = request.Key;
            string providerKey = request.Value;

            var signInResult = await _signInManager.ExternalLoginSignInAsync(
                loginProvider, providerKey, false);

            if (signInResult.Succeeded)
            {
                var user = await _userManager.FindByLoginAsync(loginProvider, providerKey);
                string token = await this.GetToken(user);

                return new LoginResponse()
                {
                    StatusCode = 200,
                    AccessToken = token,
                    User = user
                };
            }

            return new LoginResponse() { StatusCode = 401 };
        }

        // DELETE: api/ApiWithActions/5
        [HttpPost("signout")]
        public async Task SignOut()
        {
            await _signInManager.SignOutAsync();
        }
    }
}
