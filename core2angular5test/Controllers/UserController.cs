using System;
using System.Threading.Tasks;
using core2angular5test.Data;
using core2angular5test.Data.Models;
using core2angular5test.ViewModels;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace core2angular5test.Controllers
{
    [Route("api/[controller]")]
    public class UserController : BaseApiController
    {
        #region constructor
        public UserController(
            ApplicationDbContext context,
            RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration) :
            base(context, roleManager, userManager, configuration)
        {
            
        }
        #endregion
        
        #region RESTful convention methods
        /// <summary>
        /// PUT api/user
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Creates a user and returns it</returns>
        [HttpPut]
        public async Task<IActionResult> Add([FromBody] UserViewModel model)
        {
            // return a generic 500 error if the client payload is invalid
            if (model == null)
                return new StatusCodeResult(500);
            
            // check if the username/email already exists
            // TODO: tego nie powinna zapewniać walidacja asp .net identity?
            ApplicationUser user = await UserManager.FindByNameAsync(model.UserName);

            if (user != null)
                return BadRequest("Username already exists");

            user = await UserManager.FindByEmailAsync(model.Email);
            if (user != null)
                return BadRequest("Email already exists");

            var now = DateTime.Now;

            user = new ApplicationUser()
            {
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.UserName,
                Email = model.Email,                
                DisplayName = model.DisplayName,
                CreatedDate = now,
                LastModifiedDate = now
            };

            await UserManager.CreateAsync(user, model.Password);

            await UserManager.AddToRoleAsync(user, "RegisteredUser");

            user.EmailConfirmed = true;
            user.LockoutEnabled = false;

            DbContext.SaveChanges();
            
            return Json(user.Adapt<UserViewModel>(), JsonSettings);
        }
        
        #endregion
    }
}