using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Celia.io.Core.Auths.Abstractions;
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

        // GET: api/Roles/list
        [HttpGet("list")]
        public async Task<IEnumerable<ApplicationUser>> List([FromQuery] UserSort userSort)
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

            return users.Skip((userSort.PageIndex - 1) * pageSize2).Take(pageSize2);
        }

        [HttpGet("findbyid")]
        public async Task<ApplicationUser> FindByUserId([FromQuery] [Required] string userid)
        {
            return await _userManager.FindByIdAsync(userid);
        }

        [HttpGet("findbyname")]
        public async Task<ApplicationUser> FindByUserName([FromQuery] [Required] string name)
        {
            return await _userManager.FindByNameAsync(name);
        }

        [HttpGet("findbyemail")]
        public async Task<ApplicationUser> FindByEmail([FromQuery] [Required] string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        // GET api/values
        [HttpPost("adduser")]
        public async Task<ApplicationUser> AddUser([FromBody] ApplicationUser user)
        {
            var result = await _userManager.CreateAsync(user);
            if (result.Succeeded)
                return user;
            return null;
        }

        [HttpPost("resetpassword")]
        public async Task<string> ResetUserPassword([FromBody] KVRequest request)
        {
            var user = await _userManager.FindByIdAsync(request.Key);
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, request.Value);
            if (result.Succeeded)
                return request.Value;

            return string.Empty;
        }

        // GET api/values
        [HttpPost("updateuser")]
        public async Task<ApplicationUser> UpdateUser([FromBody] ApplicationUser user)
        {
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
                return user;
            return null;
        }

        // GET api/values
        [HttpPost("deleteuser")]
        public async Task<ApplicationUser> DeleteUser([FromBody] ApplicationUser user)
        {
            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
                return user;
            return null;
        } 

        [HttpGet("getrolesbyuserid")]
        public async Task<IEnumerable<ApplicationRole>> GetRolesByUserId([FromQuery] [Required] string userId)
        {
            var result = await _userManager.GetRolesAsync(new ApplicationUser() { Id = userId });
            if (result != null && result.Count > 0)
            {
                return (from one in result
                        select new ApplicationRole() { Name = one });
            }

            return new ApplicationRole[] { };
        }
    }
}