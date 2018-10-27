using System;
using System.Collections.Generic;
using System.Text;
using Celia.io.Core.Auths.Abstractions;

namespace Celia.io.Core.Auths.SDK
{
    public class RoleManagementDemo
    {
        public RoleManagementDemo(RoleManager roleManager)
        {
            //this.SignInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
            //this.UserManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            this.RoleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
        }

        public SignInManager SignInManager { get; private set; }
        public UserManager UserManager { get; private set; }
        public RoleManager RoleManager { get; private set; }

        public async void ManageRoles()
        {
            //1. 系统初始化添加角色
            ApplicationRole role = await RoleManager.AddRoleAsync("唯一角色名");
            //2. 查找角色
            role = await RoleManager.FindRoleByIdAsync("角色ID");
            role = await RoleManager.FindRoleByNameAsync("唯一角色名");

            //3. 添加权限到角色
            await RoleManager.AddRoleClaimAsync(role.Id, "微信授权用户", "读取用户信息");
            //或者
            ApplicationRoleClaim roleClaim = await RoleManager.AddRoleClaimAsync(
                role.Id, claimType: "微信授权用户", claimValue: "读取用户信息");

            //或者批量添加
            await RoleManager.AddClaimsToRoleAsync(new ApplicationRoleClaim[] { roleClaim });

            //4. 删除角色权限
            await RoleManager.RemoveRoleClaimAsync(roleClaim);
            //或者批量删除
            await RoleManager.RemoveRoleClaimsAsync(new ApplicationRoleClaim[] { roleClaim });
        }
    }
}