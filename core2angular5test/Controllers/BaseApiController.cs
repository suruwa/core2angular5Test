using core2angular5test.Data;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace core2angular5test.Controllers
{
    public class BaseApiController : Controller
    {
        #region constructor
        public BaseApiController(ApplicationDbContext context)
        {
            // Instantiate the ApplicationDbContext through DI
            DbContext = context;
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
        #endregion
    }
}