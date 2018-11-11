using core2angular5test.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using core2angular5test.Data;
using core2angular5test.Data.Models;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace core2angular5test.Controllers
{
    [Route("api/[controller]")]
    public class QuestionController : BaseApiController
    {                        
        #region Constructor
        public QuestionController(ApplicationDbContext context,
            RoleManager<IdentityRole> roleManager, 
            UserManager<ApplicationUser> userManager, 
            IConfiguration configuration) 
            : base(context, roleManager, userManager, configuration)
        {
            
        }
        #endregion
        
        #region restful convention methods

        /// <summary>
        /// Retrieves the Question with the given {id}
        /// </summary>
        /// <param name="id">The ID of an existing Question</param>
        /// <returns>the Question with the given {id}</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var question = await DbContext.Questions
                .FirstOrDefaultAsync(i => i.Id == id);
            // handle requests asking for non-existing questions
            if (question == null)
            {
                return NotFound(new
                {
                    Error = String.Format("Question ID {0} has not been found", id)
                });
            }

            return new JsonResult(
                question.Adapt<QuestionViewModel>(),JsonSettings);
        }

        /// <summary>
        /// Adds a new Question to the Database
        /// </summary>
        /// <param name="model">The QuestionViewModel containing the data to insert</param>
        [HttpPut]
        [Authorize]
        public async Task<IActionResult> Put([FromBody] QuestionViewModel model)
        {
            // return a generic HTTP Status 500 (Server Error)
            // if the client payload is invalid.
            if (model == null) return new StatusCodeResult(500);
            
            // map the ViewModel to the Model
            var question = model.Adapt<Question>();
            
            // override those properties
            //   that should be set from the server-side only
            question.QuizId = model.QuizId;
            question.Text = model.Text;
            question.Notes = model.Notes;
            // properties set from server-side
            question.CreatedDate = DateTime.Now;
            question.LastModifiedDate = question.CreatedDate;
            // add the new question
            DbContext.Questions.Add(question);
            // persist the changes into the Database.
            await DbContext.SaveChangesAsync();
            
            // return the newly-created Question to the client.
            return new JsonResult(
                question.Adapt<QuestionViewModel>(),JsonSettings);
        }

        /// <summary>
        /// Edit the Question with the given {id}
        /// </summary>
        /// <param name="model">The QuestionViewModel containing the data to update</param>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Post([FromBody] QuestionViewModel model)
        {
            // return a generic HTTP Status 500 (Server Error)
            // if the client payload is invalid.
            if (model == null) return new StatusCodeResult(500);
            
            // retrieve the question to edit
            var question = await DbContext.Questions.FirstOrDefaultAsync(q => q.Id == model.Id);
            // handle requests asking for non-existing questions
            if (question == null)
            {
                return NotFound(new
                {
                    Error = String.Format("Question ID {0} has not been found", model.Id)
                }); }
            // handle the update (without object-mapping)
            //   by manually assigning the properties
            //   we want to accept from the request
            question.QuizId = model.QuizId;
            question.Text = model.Text;
            question.Notes = model.Notes;
            // properties set from server-side
            question.LastModifiedDate = question.CreatedDate;
            // persist the changes into the Database.
            await DbContext.SaveChangesAsync();
            // return the updated Quiz to the client.
            return new JsonResult(question.Adapt<QuestionViewModel>(),JsonSettings);
        }

        /// <summary>
        /// Deletes the Question with the given {id} from the Database
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            // retrieve the question from the Database
            var question = await DbContext.Questions
                .FirstOrDefaultAsync(i => i.Id == id);
            // handle requests asking for non-existing questions
            if (question == null)
            {
                return NotFound(new
                {
                    Error = String.Format("Question ID {0} has not been found", id)
                });
            }
            // remove the quiz from the DbContext.
            DbContext.Questions.Remove(question);
            // persist the changes into the Database.
            await DbContext.SaveChangesAsync();
            // return an HTTP Status 200 (OK).
            return new OkResult();
        }
        #endregion
        
        // GET api/question/all
        [HttpGet("All/{quizId}")]
        public async Task<IActionResult> All(int quizId)
        {
            var questions = await DbContext.Questions
                .Where(q => q.QuizId == quizId)
                .ToArrayAsync();
            return new JsonResult(
                questions.Adapt<QuestionViewModel[]>(), JsonSettings);
        }
    }
}
