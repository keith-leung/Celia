using Celia.io.Core.Auths.Abstractions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Celia.io.Core.Auths.SDK
{
    public class RoleManager : ApiHelperBase
    {
        public RoleManager(string appId, string appSecret, string authApiHost)
            : base(appId, appSecret, authApiHost)
        {
        }

        /// <summary>
        /// 根据一个用户的Id，获取对应的所有角色
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<ApplicationRole>> GetRolesByUserIdAsync(string userId)
        {
            return null;
        }

        /// <summary>
        /// 更新一个角色
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public async Task<ApplicationRole> UpdateRoleAsync(ApplicationRole role)
        {
            return null;
        }

        /// <summary>
        /// 添加一个角色
        /// </summary>
        /// <param name="roleName">角色名</param>
        /// <returns></returns>
        public async Task<ApplicationRole> AddRoleAsync(string roleName)
        {
            ApplicationRole role = new ApplicationRole()
            {
                Name = roleName
            };
            JObject response = await this.HttpPostAsync("api/roles/create", JObject.FromObject(role));
            if (response != null)
            {
                return JsonConvert.DeserializeObject<ApplicationRole>(response.ToString());
            }
            return null;
        }

        /// <summary>
        /// 删除一个角色
        /// </summary>
        /// <param name="roleId">角色ID</param>
        public async Task RemoveRoleAsync(string roleId)
        {
        }

        /// <summary>
        /// 根据角色ID查找角色
        /// </summary>
        /// <param name="roleId">角色ID</param>
        /// <returns></returns>
        public async Task<ApplicationRole> FindRoleByIdAsync(string roleId)
        {
            JObject response = await this.HttpGetAsync("api/roles/findbyid?roleId=" + roleId);
            if (response != null)
            {
                return JsonConvert.DeserializeObject<ApplicationRole>(response.ToString());
            }
            return null;
        }

        /// <summary>
        /// 根据角色名查找角色
        /// </summary>
        /// <param name="roleName">角色名</param>
        /// <returns></returns>
        public async Task<ApplicationRole> FindRoleByNameAsync(string roleName)
        {
            JObject response = await this.HttpGetAsync("api/roles/findbyname?roleName=" + roleName);
            if (response != null)
            {
                return JsonConvert.DeserializeObject<ApplicationRole>(response.ToString());
            }
            return null;
        }

        /// <summary>
        /// 添加权限（Claims）到某个角色
        /// </summary>
        /// <param name="claims">权限列表</param>
        /// <returns></returns>
        public async Task<IEnumerable<ApplicationRoleClaim>> AddClaimsToRoleAsync(
            IEnumerable<ApplicationRoleClaim> claims)
        {
            JArray array = new JArray();
            foreach (var c in claims)
            {
                array.Add(JObject.FromObject(c));
            }

            JObject response = await this.HttpPostAsync("api/roles/addclaimstorole", array);
            if (response != null)
            {
                return JsonConvert.DeserializeObject<IEnumerable<ApplicationRoleClaim>>(
                    response.Value<string>("result"));
            }
            return null;
        }

        /// <summary>
        /// 添加单个权限到角色
        /// </summary>
        /// <param name="roleId">角色ID</param>
        /// <param name="claimType">权限类型</param>
        /// <param name="claimValue">权限值</param>
        /// <returns></returns>
        public Task<ApplicationRoleClaim> AddRoleClaimAsync(string roleId, string claimType, string claimValue)
        {
            return Task.FromResult<ApplicationRoleClaim>(null);

            //return this.AddRoleClaim(new ApplicationRoleClaim()
            //{
            //    //Id = IdGenerator
            //    RoleId = roleId,
            //    ClaimType = claimType,
            //    ClaimValue = claimValue,
            //});
        }

        /// <summary>
        /// 删除一个角色的权限
        /// </summary>
        /// <param name="roleClaim"></param>
        public Task RemoveRoleClaimAsync(ApplicationRoleClaim roleClaim)
        {
            return null;
        }

        /// <summary>
        /// 删除一个角色的多个权限
        /// </summary>
        /// <param name="claims"></param>
        public Task RemoveRoleClaimsAsync(IEnumerable<ApplicationRoleClaim> claims)
        {
            return null;
        }

        /// <summary>
        /// 根据角色ID，获取对应的所有权限
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<ApplicationRoleClaim>> GetClaimsByRoleIdAsync(string roleId)
        {
            JObject response = await this.HttpGetAsync("api/roles/getclaimsbyroleid?roleId=" + roleId);
            if (response != null)
            {
                return JsonConvert.DeserializeObject<IEnumerable<ApplicationRoleClaim>>(
                    response.Value<string>("result").ToString());
            }
            return null;
        }

        /// <summary>
        /// 判断用户是否属于某个角色
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="roleId">角色ID</param>
        /// <returns></returns>
        public async Task<bool> IsInRoleAsync(string userId, string roleName)
        {
            JObject response = await this.HttpGetAsync($"api/userroles/isinrole?userId={userId}&roleName={roleName}");
            if (response != null)
            {
                return response.Value<bool>("result");
            }

            return false;
        }
    }
}
