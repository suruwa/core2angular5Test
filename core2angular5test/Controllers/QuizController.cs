using core2angular5test.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using core2angular5test.Controllers;
using core2angular5test.Data;
using core2angular5test.Data.Models;
using Mapster;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace core2angular5test
{
    [Route("api/[controller]")]
    public class QuizController : BaseApiController
    {
        /*
         * Dummy data providers, TDD
         * Moq, NMock3, NSubstitute, or Rhino          
         */               
        
        #region constructor

        public QuizController(ApplicationDbContext context) : base(context)
        {
            
        }
        
        #endregion

        #region RESTful conventions methods
        /// <summary>
        /// GET: api/quiz/{id}
        /// Retrieves the Quiz with the given {id}
        /// </summary>
        /// <param name="id">The ID of an existing Quiz</param>
        /// <returns>the Quiz with the given {id}</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var quiz = await DbContext.Quizzes.FirstOrDefaultAsync(q => q.Id == id);
            
            // handle requests asking for non-existing quizzes
            if (quiz == null)
            {
                return NotFound(new
                {
                    Error = string.Format("Quiz ID {0} has not been found", id)
                });
            }
            
            return new JsonResult(quiz.Adapt<QuizViewModel>(), JsonSettings);                       
        }

        /// <summary>
        /// Adds a new Quiz to the Database
        /// </summary>
        /// <param name="model">The QuizViewModel containing the data to insert</param>
        [HttpPut] 
        public async Task<IActionResult> Put([FromBody] QuizViewModel model)
        {
            // return a generic HTTP Status 500 (Server Error)
            // if the client payload is invalid.
            if (model == null) 
                return new StatusCodeResult(500);
            
            // handle the insert (without object-mapping)
            var quiz = new Quiz();
            // properties taken from the request
            quiz.Title = model.Title;
            quiz.Description = model.Description;
            quiz.Text = model.Text;
            quiz.Notes = model.Notes;
            // properties set from server-side
            quiz.CreatedDate = DateTime.Now;
            quiz.LastModifiedDate = quiz.CreatedDate;
            
            
            // Set a temporary author using the Admin user's userId
            // as user login isn't supported yet: we'll change this later on.
            quiz.UserId = (await DbContext.Users
                .FirstOrDefaultAsync(u => u.UserName == "Admin")).Id;
            // add the new quiz
            DbContext.Quizzes.Add(quiz);
            
            // persist the changes into the Database.
            await DbContext.SaveChangesAsync();
            // return the newly-created Quiz to the client.
            return new JsonResult(quiz.Adapt<QuizViewModel>(), JsonSettings);
        }

        /// <summary>
        /// Edit the Quiz with the given {id}
        /// </summary>
        /// <param name="model">The QuizViewModel containing the data to update</param>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] QuizViewModel model)
        {
            // return a generic HTTP Status 500 (Server Error)
            // if the client payload is invalid.
            if (model == null) return new StatusCodeResult(500);
            // retrieve the quiz to edit

            var quiz = await DbContext.Quizzes.FirstOrDefaultAsync(q => q.Id == model.Id);
            // handle requests asking for non-existing quizzes
            if (quiz == null)
            {
                return NotFound(new
                {
                    Error = String.Format("Quiz ID {0} has not been found",
                        model.Id)
                });
            }

            // handle the update (without object-mapping)
            //   by manually assigning the properties
            //   we want to accept from the request
            quiz.Title = model.Title;
            quiz.Description = model.Description;
            quiz.Text = model.Text;
            quiz.Notes = model.Notes;
            // properties set from server-side
            quiz.LastModifiedDate = quiz.CreatedDate;
            // persist the changes into the Database.
            await DbContext.SaveChangesAsync();

            // return the updated Quiz to the client.
            return new JsonResult(quiz.Adapt<QuizViewModel>(), JsonSettings);
        }

        /// <summary>
        /// Deletes the Quiz with the given {id} from the Database
        /// </summary>
        /// <param name="id">The ID of an existing Test</param>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            // retrieve the quiz from the Database
            var quiz = await DbContext.Quizzes
                .FirstOrDefaultAsync(i => i.Id == id);
            // handle requests asking for non-existing quizzes
            if (quiz == null)
            {
                return NotFound(new
                {
                    Error = String.Format("Quiz ID {0} has not been found", id)
                });
            }

            // remove the quiz from the DbContext.
            DbContext.Quizzes.Remove(quiz);
            // persist the changes into the Database.
            await DbContext.SaveChangesAsync();
            // return an HTTP Status 200 (OK).
            return new OkResult();
        }

        #endregion


        #region Attribute-based routing methods
        /// <summary>
        /// GET: api/quiz/latest
        /// Retrieves the {num} latest Quizzes
        /// </summary>
        /// <param name="num">the number of quizzes to retrieve</param>
        /// <returns>the {num} latest Quizzes</returns>
        // GET api/quiz/latest
        [HttpGet("latest/{num}")]
        public async Task<IActionResult> Latest(int num = 10)
        {
            var latest = await DbContext.Quizzes
                .OrderByDescending(q => q.CreatedDate)
                .Take(num)
                .ToArrayAsync();
            // output the result in JSON format
            return new JsonResult(
                latest.Adapt<QuizViewModel[]>(), JsonSettings);
        }

        /// <summary>
        /// GET: api/quiz/ByTitle
        /// Retrieves the {num} Quizzes sorted by Title (A to Z)
        /// </summary>
        /// <param name="num">the number of quizzes to retrieve</param>
        /// <returns>{num} Quizzes sorted by Title</returns>
        [HttpGet("ByTitle/{num:int?}")]
        public async Task<IActionResult> ByTitle(int num = 10)
        {
            var byTitle = await DbContext.Quizzes
                .OrderBy(q => q.Title)
                .Take(num)
                .ToArrayAsync();

            return new JsonResult(
                byTitle.Adapt<QuizViewModel[]>(), JsonSettings);
        }

        /// <summary>
        /// GET: api/quiz/mostViewed
        /// Retrieves the {num} random Quizzes
        /// </summary>
        /// <param name="num">the number of quizzes to retrieve</param>
        /// <returns>{num} random Quizzes</returns>
        [HttpGet("Random/{num:int?}")]
        public async Task<IActionResult> Random(int num = 10)
        {
            var random = await DbContext.Quizzes
                .OrderBy(q => Guid.NewGuid())
                .Take(num)
                .ToArrayAsync();
            
            return new JsonResult(
                random.Adapt<QuizViewModel[]>(), JsonSettings);
        }
        #endregion
    }
}
