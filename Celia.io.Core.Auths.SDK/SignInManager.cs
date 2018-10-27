using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Celia.io.Core.Auths.SDK
{
    public class SignInManager : ApiHelperBase
    {
        public SignInManager(string appId, string appSecret, string authApiHost)
            : base(appId, appSecret, authApiHost)
        {
        }

        /// <summary>
        /// 通过Email登录
        /// </summary>
        /// <param name="email">Email</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        public async Task<LoginResponse> LoginByEmailAsync(string email, string password)
        {
            JObject kvRequest = new JObject();
            kvRequest.Add("Key", email);
            kvRequest.Add("Value", password);
            JObject response = await this.HttpPostAsync("api/signin/loginbyemail", kvRequest);

            if (response != null)
            {
                LoginResponse loginResponse = JsonConvert.DeserializeObject<LoginResponse>(response.ToString());

                return loginResponse;
            }

            return null;
        }

        /// <summary>
        /// 通过用户名密码登录
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        public async Task<LoginResponse> LoginByUserNameAsync(string username, string password)
        {
            JObject kvRequest = new JObject();
            kvRequest.Add("Key", username);
            kvRequest.Add("Value", password);
            JObject response = await this.HttpPostAsync("api/signin/loginbyusername", kvRequest);

            if (response != null)
            {
                LoginResponse loginResponse = JsonConvert.DeserializeObject<LoginResponse>(response.ToString());

                return loginResponse;
            }

            return null;
        }

        /// <summary>
        /// 通过用户ID密码登录
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        public async Task<LoginResponse> LoginByUserIdAsync(string userId, string password)
        {
            JObject kvRequest = new JObject();
            kvRequest.Add("Key", userId);
            kvRequest.Add("Value", password);
            JObject response = await this.HttpPostAsync("api/signin/loginbyuserid", kvRequest);

            if (response != null)
            {
                LoginResponse loginResponse = JsonConvert.DeserializeObject<LoginResponse>(response.ToString());

                return loginResponse;
            }

            return null;
        }

        /// <summary>
        /// 外部登录
        /// </summary>
        /// <param name="loginProvider">登录提供方</param>
        /// <param name="providerKey">登录提供方给出的键值（唯一ID）</param>
        /// <returns></returns>
        public Task<LoginResponse> ExternalLoginAsync(string loginProvider, string providerKey)
        {
            return null;
        }

        /// <summary>
        /// 重设密码
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="newPassword"></param>
        public Task ResetPasswordAsync(string userId, string newPassword)
        {
            return null;
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="oldPassword">旧密码</param>
        /// <param name="newPassword">新密码</param>
        public Task ChangePasswordAsync(string userId, string oldPassword, string newPassword)
        {
            return null;
        }

        /// <summary>
        /// 退出登录
        /// </summary>
        /// <param name="userId"></param>
        public Task LogoutAsync(string userId)
        {
            return null;
        }

        /// <summary>
        /// 校验AccessToken
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public async Task<TokenResponse> AccessTokenAsync(string userId, string accessToken)
        {
            JObject response = await this.TokenHttpGetAsync(accessToken, $"api/signin/validatetoken?token={accessToken}");

            if (response != null)
            {
                TokenResponse loginResponse = JsonConvert.DeserializeObject<TokenResponse>(response.ToString());

                return loginResponse;
            }

            return null;
        }

        /// <summary>
        /// 刷新AccessToken，根据一个合法AccessToken重新生成一个有效期更久的AccessToken
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public async Task<TokenResponse> RefreshTokenAsync(string userId, string accessToken)
        {
            JObject response = await this.TokenHttpGetAsync(accessToken, $"api/signin/refreshtoken?token={accessToken}");

            if (response != null)
            {
                TokenResponse loginResponse = JsonConvert.DeserializeObject<TokenResponse>(response.ToString());

                return loginResponse;
            }

            return null;
        }
    }
}

