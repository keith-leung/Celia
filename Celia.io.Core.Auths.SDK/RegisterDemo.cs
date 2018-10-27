using Celia.io.Core.Auths.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Celia.io.Core.Auths.SDK
{
    class RegisterDemo
    {
        public RegisterDemo(SignInManager signInManager, UserManager userManager, RoleManager roleManager)
        {
            this.SignInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
            this.UserManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            this.RoleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
        }

        public SignInManager SignInManager { get; private set; }
        public UserManager UserManager { get; private set; }
        public RoleManager RoleManager { get; private set; }

        public async void RegisterAndLoginByUserNamePassword()
        {
            string userid = string.Empty, username = string.Empty, email = string.Empty, userpassword = string.Empty;

            //1. 创建一个用户
            ApplicationUser user = await UserManager.CreateUserAsync(new ApplicationUser()
            {
                Email = email,//Email，必须，如果没有，则生成一个随机值或者组合一个有意义的值
                UserName = username,//用户名，必须，如果没有，则生成一个随机值或者组合一个有意义的值
                Id = userid,//如果自行指定ID，则会根据这个ID来生成用户，但需要调用方保证ID不冲突，否则系统自动生成
            });

            //2. （只使用微信登录可掠过这一步）未设置密码的用户设置用户密码，必须把UserId带过来
            await SignInManager.ResetPasswordAsync(user.Id, userpassword);

            //3. （微信登录）添加一个LoginType
            ApplicationUserLogin userLogin = await UserManager.AddUserLoginAsync(new ApplicationUserLogin()
            {
                LoginProvider = "微信",//设置一个唯一的常量
                ProviderDisplayName = "微信", //展示用
                ProviderKey = "${OPEN_ID}", //把微信OpenID写上
                UserId = user.Id, //用户ID
                                  //如果自行指定ID，则会根据这个ID来生成用户，但需要调用方保证ID不冲突，否则系统自动生成
            });

            //4. 添加用户到某个角色
            ApplicationUserRole userRole = await UserManager.AddUserRoleAsync(userid, "患者");
            //或者new一个ApplicationUserRole
            await UserManager.AddUserRoleAsync(userRole);
            //批量方法
            await UserManager.AddUserRolesAsync(new ApplicationUserRole[] { userRole });

            //5. （非外部来源）登录，随便选一个
            LoginResponse loginResponse = await SignInManager.LoginByEmailAsync(email, userpassword);
            loginResponse = await SignInManager.LoginByUserIdAsync(userid, userpassword);
            loginResponse = await SignInManager.LoginByUserNameAsync(username, userpassword);

            //6. （外部来源：微信）用户登录
            userLogin = await UserManager.GetLoginByUserIdLoginTypeAsync(userid, "微信", "${OPEN_ID}");
            loginResponse = await SignInManager.ExternalLoginAsync(userLogin.LoginProvider, userLogin.ProviderKey);

            //7. 外部登录，去除绑定
            await UserManager.RemoveUserLoginAsync(userLogin);
            //8. 删除角色
            await UserManager.RemoveUserRoleAsync(userRole);
        }
    }
}
