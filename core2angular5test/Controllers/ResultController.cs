using System;
using System.Collections.Generic;
using System.Linq;
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
using Newtonsoft.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace core2angular5test.Controllers
{
    
        [Route("api/[controller]")]
        public class ResultController : BaseApiController
        {            
            #region Constructor
            public ResultController(
                ApplicationDbContext context,
                RoleManager<IdentityRole> roleManager, 
                UserManager<ApplicationUser> userManager, 
                IConfiguration configuration) 
                : base(context, roleManager, userManager, configuration)
            {
                // Instantiate the ApplicationDbContext through DI                
            }
            #endregion

            #region RESTful convention methods

            /// <summary>
            /// Retrieves the Result with the given {id}
            /// </summary>
            /// <param name="id">The ID of an existing Result</param>
            /// <returns>the Result with the given {id}</returns>
            [HttpGet("{id}")]
            public async Task<IActionResult> Get(int id)
            {
                var result = await DbContext.Results
                    .FirstOrDefaultAsync(i => i.Id == id);
                // handle requests asking for non-existing results
                if (result == null)
                {
                    return NotFound(new
                    {
                        Error = String.Format("Result ID {0} has not been found", id)
                    }); }
                return new JsonResult(
                    result.Adapt<ResultViewModel>(), JsonSettings);
            }

            /// <summary>
            /// Adds a new Result to the Database
            /// </summary>
            /// <param name="model">The ResultViewModel containing the data to insert</param>
            [HttpPut]
            [Authorize]
            public async Task<IActionResult> Put([FromBody] ResultViewModel model)
            {
                // return a generic HTTP Status 500 (Server Error)
                // if the client payload is invalid.
                if (model == null) return new StatusCodeResult(500);
                // map the ViewModel to the Model
                var result = model.Adapt<Result>();
                // override those properties
                // that should be set from the server-side only
                result.CreatedDate = DateTime.Now;
                result.LastModifiedDate = result.CreatedDate;
                
                // add the new result
                DbContext.Results.Add(result);
                // persist the changes into the Database.
                await DbContext.SaveChangesAsync();
                // return the newly-created Result to the client.
                return new JsonResult(result.Adapt<ResultViewModel>(), JsonSettings);
            }

            /// <summary>
            /// Edit the Result with the given {id}
            /// </summary>
            /// <param name="model">The ResultViewModel containing the data to update</param>
            [HttpPost]
            [Authorize]
            public async Task<IActionResult> Post([FromBody] ResultViewModel model)
            {
                // return a generic HTTP Status 500 (Server Error)
                // if the client payload is invalid.
                if (model == null) return new StatusCodeResult(500);
                // retrieve the result to edit
                var result = await DbContext.Results.FirstOrDefaultAsync(q => q.Id == model.Id);
                // handle requests asking for non-existing results
                if (result == null)
                {
                    return NotFound(new
                    {
                        Error = String.Format("Result ID {0} has not been found", model.Id)
                    }); }
                // handle the update (without object-mapping)
                // by manually assigning the properties
                // we want to accept from the request
                result.QuizId = model.QuizId;
                result.Text = model.Text;
                result.MinValue = model.MinValue;
                result.MaxValue = model.MaxValue;
                result.Notes = model.Notes;
                
                // properties set from server-side
                result.LastModifiedDate = result.CreatedDate;
                // persist the changes into the Database.
                DbContext.SaveChanges();
                // return the updated Quiz to the client.
                return new JsonResult(result.Adapt<ResultViewModel>(), JsonSettings);
            }
            
            /// <summary>
            /// Deletes the Result with the given {id} from the Database
            /// </summary>
            /// <param name="id">The ID of an existing Result</param>
            [HttpDelete("{id}")]
            [Authorize]
            public async Task<IActionResult> Delete(int id)
            {
                // retrieve the result from the Database
                var result = await DbContext.Results.FirstOrDefaultAsync(i => i.Id == id);
                // handle requests asking for non-existing results
                if (result == null)
                {
                    return NotFound(new
                    {
                        Error = String.Format("Result ID {0} has not been found", id)
                    });
                }
                // remove the quiz from the DbContext.
                DbContext.Results.Remove(result);
                // persist the changes into the Database.
                DbContext.SaveChanges();
                // return an HTTP Status 200 (OK).
                return new OkResult();
            }
            #endregion
            
            // GET api/question/all
            [HttpGet("All/{quizId}")]
            public async Task<IActionResult> All(int quizId)
            {
                var results = await DbContext.Results
                    .Where(q => q.QuizId == quizId)
                    .ToArrayAsync();
                return new JsonResult(
                    results.Adapt<ResultViewModel[]>(), JsonSettings);
            }
        }
    
}
