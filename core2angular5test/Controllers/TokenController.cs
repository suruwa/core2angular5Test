using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using core2angular5test.Data;
using core2angular5test.Data.Models;
using core2angular5test.ViewModels;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace core2angular5test.Controllers
{
    [Route("api/auth")]
    public class TokenController : BaseApiController
    {
        #region Private members
        #endregion
        
        #region Constructor
        public TokenController(
            ApplicationDbContext context,
            RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration
        ) : base(context, roleManager, userManager, configuration)
        {
            
        }
        #endregion

        [AllowAnonymous]
        [HttpPost("jwt")]
        public async Task<IActionResult> Jwt([FromBody] TokenRequestViewModel model)
        {
            // return a generic HTTP Status 500 (Server error)
            // if th client payload is invalid
            if (model == null)
                return new StatusCodeResult(500);

            switch (model.grant_type)
            {
                case "password":
                    return await GetToken(model);
                case "refresh_token":
                    return await GetRefreshToken(model);
                default:
                    // not support - return a HTTP 401 (Unauthorized)
                    return new UnauthorizedResult();
            }
        }
    
        private async Task<IActionResult> GetToken(TokenRequestViewModel model)
        {
            try
            {
                // check if there's a user with the given username
                var user = await UserManager.FindByNameAsync(model.username);
                
                // fallback to support e-mail address instead of username
                if (user == null && model.username.Contains("@"))
                    user = await UserManager.FindByEmailAsync(model.username);

                if (user == null || !await UserManager.CheckPasswordAsync(user, model.password))
                { 
                    // user does not exist or password mismatch
                    return new UnauthorizedResult();
                }
                
                // username and password matches - create and return the JWT, with refresh token

                var rt = CreateRefreshToken(model.client_id, user.Id);
                
                // add the new refresh token to the DB
                DbContext.Tokens.Add(rt);
                await DbContext.SaveChangesAsync();

                var response = CreateAccessToken(user.Id, rt.Value);

                return Json(response);
            }
            catch
            {
                return new UnauthorizedResult();                
            }
        }

        private async Task<IActionResult> GetRefreshToken(TokenRequestViewModel model)
        {
            try
            {
                // check if the received refresh token exists for the given client id
                var rt = await DbContext.Tokens.FirstOrDefaultAsync(t => 
                    t.ClientId == model.client_id &&
                    t.Value == model.refresh_token);

                if (rt == null)
                {
                    // refresh token not found or invalid (or invalid client Id)
                    return new UnauthorizedResult();                    
                }
                
                // check if there's a user the refresh token's userId
                var user = await UserManager.FindByIdAsync(rt.UserId);

                if (user == null)
                {
                    // UserId not found or invalid
                    return new UnauthorizedResult();                    
                }
                
                // generate a new refresh token
                var rtNew = CreateRefreshToken(model.client_id, user.Id);
                
                // invalidate the old refresh token
                DbContext.Tokens.Remove(rt);
                
                // add the new refresh token
                DbContext.Tokens.Add(rtNew);
                
                // persist changes in the DB
                await DbContext.SaveChangesAsync();
                
                // create a new access token
                var response = CreateAccessToken(user.Id, rtNew.Value);
                
                // send it to the client
                return Json(response);
            }   
            catch
            {
                return new UnauthorizedResult();                
            }
        }

        private Token CreateRefreshToken(string clientId, string userId)
        {
            return new Token()
            {
                ClientId = clientId,
                UserId = userId,
                Type = 0,
                Value = Guid.NewGuid().ToString("N"),
                CreatedDate = DateTime.UtcNow
            };
        }

        private TokenResponseViewModel CreateAccessToken(string userId, string refreshToken)
        {
            DateTime now = DateTime.UtcNow;
            
            // add the registered claims for JWT (RFC7519)
            // for more info, see: https://tools.ietf.org/html/rfc7519#section-4.1
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, new DateTimeOffset(now).ToUnixTimeSeconds().ToString()),
                // TODO: add additional claims here                                          
            };

            var tokenExpirationMins = Configuration.GetValue<int>("Auth:Jwt:TokenExpirationInMinutes");
            var issuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Auth:Jwt:Key"]));
                
            var token = new JwtSecurityToken(
                issuer: Configuration["Auth:Jwt:Issuer"],
                audience: Configuration["Auth:Jwt:Audience"],
                claims: claims,
                notBefore: now,
                expires: now.Add(TimeSpan.FromMinutes(tokenExpirationMins)),
                signingCredentials: new SigningCredentials(issuerSigningKey, SecurityAlgorithms.HmacSha256)
            );

            var encodedToken = new JwtSecurityTokenHandler().WriteToken(token);
                
            // build and return the response
            var response = new TokenResponseViewModel()
            {
                token = encodedToken,
                expiration = tokenExpirationMins,
                refresh_token = refreshToken
            };

            return response;
        }
    }
}


