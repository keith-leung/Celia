using System;
using System.Collections.Generic;
using System.Text;

namespace Celia.io.Core.Auths.SDK
{
    class SignInDemo
    {
        public SignInDemo(SignInManager signInManager, UserManager userManager, RoleManager roleManager)
        {
            this.SignInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
            this.UserManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            this.RoleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
        }

        public SignInManager SignInManager { get; private set; }
        public UserManager UserManager { get; private set; }
        public RoleManager RoleManager { get; private set; }

        public async void Tokens()
        {
            string token = (new LoginResponse()).AccessToken;
            //通过登录获取到Token

            //每次校验token
            TokenResponse response = await SignInManager.AccessTokenAsync(string.Empty, token);
            if (response.StatusCode == 200)
            {
                //正常，校验通过
            }
            else if (response.StatusCode == 403200) //假如这个返回码是代表Token过期
            {
                string userId = string.Empty;
                //根据用户ID刷新，用户ID可以通过ASP.NET http Context获取
                TokenResponse response2 = await SignInManager.RefreshTokenAsync(userId, token);
                var 新的token = response2.AccessToken; //要求客户端接收新的Token
            }
            else
            {
                throw new Exception("Token不合法");
            }
        }
    }
}
