using core2angular5test.Data;
using core2angular5test.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace core2angular5test.Controllers
{
    public class BaseApiController : Controller
    {
        #region constructor
        public BaseApiController(
            ApplicationDbContext context,
            RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration
        )
        {
            // Instantiate the ApplicationDbContext through DI
            DbContext = context;
            RoleManager = roleManager;
            UserManager = userManager;
            Configuration = configuration;
            
            // Instantiate a single JsonSerializerSettings object
            // that can be reused multiple times.
            JsonSettings = new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented
            };
        }
        #endregion
        
        #region Shared Properties
        protected ApplicationDbContext DbContext { get; private set; }
        protected JsonSerializerSettings JsonSettings { get; private set; }
        protected RoleManager<IdentityRole> RoleManager { get; private set; }
        protected UserManager<ApplicationUser> UserManager { get; private set; }
        protected IConfiguration Configuration { get; private set; }        
        #endregion
    }
}