using System;
using System.Collections.Generic;
using System.Text;
using Celia.io.Core.Auths.Abstractions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http; 
using System.Threading.Tasks;

namespace Celia.io.Core.Auths.SDK
{
    public class UserManager : ApiHelperBase
    {

        public UserManager(string appId, string appSecret, string authApiHost)
            : base(appId, appSecret, authApiHost)
        {
        }

        public async Task<ApplicationUser> FindUserByIdAsync(string userid)
        {
            JObject response = await this.HttpGetAsync($"api/users/findbyid?userid={userid}");

            if (response != null)
            {
                ApplicationUser user = JsonConvert.DeserializeObject<ApplicationUser>(response.ToString());

                return user;
            }

            return null;
        }

        public async Task<ApplicationUser> FindUserByUserName(string username)
        {
            JObject response = await this.HttpGetAsync($"api/users/findbyname?name={username}");

            if (response != null)
            {
                ApplicationUser user = JsonConvert.DeserializeObject<ApplicationUser>(response.ToString());

                return user;
            }

            return null;
        }

        public async Task<ApplicationUser> FindUserByEmail(string email)
        {
            JObject response = await this.HttpGetAsync($"api/users/findbyemail?email={email}");

            if (response != null)
            {
                ApplicationUser user = JsonConvert.DeserializeObject<ApplicationUser>(response.ToString());

                return user;
            }

            return null;
        }

        public async Task ResetPassword(string userid, string password)
        {
            JObject kvRequest = new JObject();
            kvRequest.Add("Key", userid);
            kvRequest.Add("Value", password);
            JObject response = await this.HttpPostAsync("api/users/resetpassword", kvRequest);
        }

        /// <summary>
        /// 创建一个User
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<ApplicationUser> CreateUserAsync(ApplicationUser user)
        {
            return Task.FromResult(user);
        }

        /// <summary>
        /// 更新一个User的信息
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<ApplicationUser> UpdateUserAsync(ApplicationUser user)
        {
            return Task.FromResult(user);
        }

        /// <summary>
        /// 删除一个用户，包含删除所有的角色关联关系/Claims
        /// </summary>
        /// <param name="userId"></param>
        public Task DeleteUserAsync(string userId)
        {
            return null;
        }

        /// <summary>
        /// 添加一个用户的账号关联登录信息。
        /// 例如微信：userLogin.LoginProvider = '微信'；userLogin.ProviderKey = ${OpenID}
        /// </summary>
        /// <param name="userLogin"></param>
        /// <returns></returns>
        public Task<ApplicationUserLogin> AddUserLoginAsync(ApplicationUserLogin userLogin)
        {
            return Task.FromResult(userLogin);
        }

        /// <summary>
        /// 去除一个用户的账号关联信息
        /// </summary>
        /// <param name="userLogin"></param>
        public Task RemoveUserLoginAsync(ApplicationUserLogin userLogin)
        {
            return null;
        }

        /// <summary>
        /// 获取一个用户账号关联信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Task<IEnumerable<ApplicationUserLogin>> GetLoginsByUserIdAsync(string userId)
        {
            return Task.FromResult<IEnumerable<ApplicationUserLogin>>(null);
        }

        /// <summary>
        /// 获取一个用户账号关联信息（判断是否存在）
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="loginProvider"></param>
        /// <param name="providerKey"></param>
        /// <returns></returns>
        public Task<ApplicationUserLogin> GetLoginByUserIdLoginTypeAsync(
            string userId, string loginProvider, string providerKey)
        {
            return null;
        }

        /// <summary>
        /// 添加一个用户到某个角色
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="roleName">角色名</param>
        /// <returns></returns>
        public async Task<ApplicationUserRole> AddUserRoleAsync(string userId, string roleName)
        {
            JObject response = await this.HttpGetAsync("api/roles/findbyname?roleName=" + roleName);

            if (response != null)
            {
                ApplicationRole role = JsonConvert.DeserializeObject<ApplicationRole>(response.ToString());

                if (role != null)
                {
                    return await this.AddUserRoleAsync(
                    new ApplicationUserRole()
                    {
                        UserId = userId,
                        RoleId = role.Id
                    }
                     );
                }
            }

            return null;
        }

        /// <summary>
        /// 添加一个用户到某个角色
        /// </summary>
        /// <param name="userRole"></param>
        /// <returns></returns>
        public async Task<ApplicationUserRole> AddUserRoleAsync(ApplicationUserRole userRole)
        {
            var response = await this.HttpPostAsync("api/userroles/adduserrole", JObject.FromObject(userRole));
            if (response != null)
                return JsonConvert.DeserializeObject<ApplicationUserRole>(response.ToString());

            return null;
        }

        /// <summary>
        /// 添加一个用户到多个角色
        /// </summary>
        /// <param name="userRoles"></param>
        /// <returns></returns>
        public Task<IEnumerable<ApplicationUserRole>> AddUserRolesAsync(IEnumerable<ApplicationUserRole> userRoles)
        {
            return Task.FromResult(userRoles);
        }

        /// <summary>
        /// 从某个角色中删除一个用户
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="roleName">角色名称</param>
        public Task RemoveUserRoleAsync(string userId, string roleName)
        {
            return null;
        }

        /// <summary>
        /// 从某个角色中删除一个用户
        /// </summary> 
        /// <param name="userRole"></param>
        public Task RemoveUserRoleAsync(ApplicationUserRole userRole)
        {
            return null;
        }

        /// <summary>
        /// 获取一个用户的所有角色
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<ApplicationRole>> GetRolesByUserIdAsync(string userId)
        {
            JObject response = await this.HttpGetAsync($"api/users/getrolesbyuserid?userId={userId}");

            if (response != null)
            {
                IEnumerable<ApplicationRole> roles = JsonConvert.DeserializeObject<
                    IEnumerable<ApplicationRole>>(
                    response.Value<string>("result").ToString());

                return roles;
            }

            return null;
        }

        /// <summary>
        /// 获取一个用户所有的权限
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        public Task<IEnumerable<ApplicationRoleClaim>> GetClaimsByUserIdAsync(string userId)
        {
            return null;
        }
    }
}