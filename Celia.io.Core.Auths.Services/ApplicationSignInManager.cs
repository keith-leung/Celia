using System;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Celia.io.Core.Auths.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Principal;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Celia.io.Core.Auths.Services
{
    public class ApplicationSignInManager : SignInManager<ApplicationUser>
    {
        private SecurityTokenHandler _securityTokenHandler;

        public ApplicationSignInManager(UserManager<ApplicationUser> userManager,
            IHttpContextAccessor contextAccessor,
            IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory, IOptions<IdentityOptions> optionsAccessor,
            ILogger<SignInManager<ApplicationUser>> logger,
            Microsoft.AspNetCore.Authentication.IAuthenticationSchemeProvider schemes)
            : base(userManager, contextAccessor, claimsFactory,
                  optionsAccessor, logger, schemes)
        {
            _securityTokenHandler = new JwtSecurityTokenHandler();
        }

        public string GetToken(string issuer, string audience, SigningCredentials credentials,
            string userid, IEnumerable<Claim> userClaims, DateTimeOffset expireOffset)
        {
            return this.RefreshToken(string.Empty,
                issuer, audience, credentials, userid, userClaims, expireOffset);
        }

        public bool ValidateToken(string sourceToken)
        {
            SecurityToken securityToken = null;
            if (!string.IsNullOrEmpty(sourceToken))
            {
                securityToken = _securityTokenHandler.ReadToken(sourceToken);
                if (securityToken == null)
                {
                    throw new ArgumentException("Token is invalid. ", "sourceToken");
                }

                if (securityToken.ValidTo.CompareTo(DateTime.Now) > 0)
                    return true;
            }

            return false;
        }

        public string RefreshToken(string sourceToken, string issuer, string audience, SigningCredentials credentials,
            string userid, IEnumerable<Claim> userClaims, DateTimeOffset expireOffset)
        {
            SecurityToken securityToken = null;
            if (!string.IsNullOrEmpty(sourceToken))
            {
                securityToken = _securityTokenHandler.ReadToken(sourceToken);
                if (securityToken == null)
                {
                    throw new ArgumentException("Token is invalid. ", "sourceToken");
                }
            }

            // Here, you should create or look up an identity for the user which is being authenticated.
            // For now, just creating a simple generic identity.
            ClaimsIdentity identity = new ClaimsIdentity(new GenericIdentity(userid, "TokenAuth"),
                userClaims);
            //new[] { new Claim("EntityID", "1", ClaimValueTypes.Integer) });

            securityToken = _securityTokenHandler.CreateToken(
                new Microsoft.IdentityModel.Tokens.SecurityTokenDescriptor()
                {
                    Issuer = issuer,
                    Audience = audience,
                    SigningCredentials = credentials,
                    Subject = identity,
                    Expires = expireOffset.UtcDateTime,
                });

            return _securityTokenHandler.WriteToken(securityToken);
        }
    }
}