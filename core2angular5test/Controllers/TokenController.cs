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
                default:
                    // not support - return a HTTP 401 (Unauthorized)
                    return new UnauthorizedResult();
            }
        }


        public async Task<IActionResult> GetToken(TokenRequestViewModel model)
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
                
                // username and password matches - create and return the JWT
                
                DateTime now = DateTime.UtcNow;
                
                // add the registered claims for JWT (RFC7519)
                // for more info, see: https://tools.ietf.org/html/rfc7519#section-4.1
                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id),
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
                    expiration = tokenExpirationMins
                };

                return Json(response);
            }
            catch
            {
                return new UnauthorizedResult();                
            }
        }
    }
}