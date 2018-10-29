using core2angular5test.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using core2angular5test.Data;
using Mapster;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace core2angular5test
{
    [Route("api/[controller]")]
    public class QuizController : Controller
    {
        /*
         * Dummy data providers, TDD
         * Moq, NMock3, NSubstitute, or Rhino          
         */
        
        #region private fields

        private ApplicationDbContext dbContext;                
        
        #endregion
        
        #region constructor

        public QuizController(ApplicationDbContext context)
        {
            // Instantiate the ApplicationDbContext through DI
            dbContext = context;
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
            var quiz = await dbContext.Quizzes.FirstOrDefaultAsync(q => q.Id == id);
            
            return new JsonResult(quiz.Adapt<QuizViewModel>(),
                new JsonSerializerSettings(){
                    Formatting = Formatting.Indented
                });                       
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
            var latest = await dbContext.Quizzes
                .OrderByDescending(q => q.CreatedDate)
                .Take(num)
                .ToArrayAsync();
            // output the result in JSON format
            return new JsonResult(
                latest.Adapt<QuizViewModel[]>(),
                new JsonSerializerSettings()
                {
                    Formatting = Formatting.Indented
                }
            );
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
            var byTitle = await dbContext.Quizzes
                .OrderBy(q => q.Title)
                .Take(num)
                .ToArrayAsync();

            return new JsonResult(
                byTitle.Adapt<QuizViewModel[]>(),
                new JsonSerializerSettings()
                {
                    Formatting = Formatting.Indented
                }
            );
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
            var random = await dbContext.Quizzes
                .OrderBy(q => Guid.NewGuid())
                .Take(num)
                .ToArrayAsync();
            
            return new JsonResult(
                random.Adapt<QuizViewModel[]>(),
                new JsonSerializerSettings() { Formatting = Formatting.Indented }
            );
        }
        #endregion
    }
}
