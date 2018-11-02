using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using core2angular5test.Data;
using core2angular5test.Data.Models;
using core2angular5test.ViewModels;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace core2angular5test.Controllers
{
    [Route("api/[controller]")]
    public class AnswerController : BaseApiController
    {        
        #region Constructor
        public AnswerController(ApplicationDbContext context) : base(context)
        {
            
        }    
        #endregion

        
        #region RESTful conventions methods
        /// <summary>
        /// Retrieves the Answer with the given {id}
        /// </summary>
        /// &lt;param name="id">The ID of an existing Answer</param>
        /// <returns>the Answer with the given {id}</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var answer = await DbContext.Answers
                .FirstOrDefaultAsync(i => i.Id == id);
            
            // handle requests asking for non-existing answers
            if (answer == null)
            {
                return NotFound(new
                {
                    Error = String.Format("Answer ID {0} has not been found", id)
                });
            }

            return new JsonResult(answer.Adapt<AnswerViewModel>(), JsonSettings);
        }
        /// <summary>
        /// Adds a new Answer to the Database
        /// </summary>
        /// <param name="model">The AnswerViewModel containing the data to insert</param>
        [HttpPut]
        public async Task<IActionResult> Put([FromBody]AnswerViewModel model)
        {
            // return a generic HTTP Status 500 (Server Error)
            // if the client payload is invalid.
            if (model == null) return new StatusCodeResult(500);
            
            // map the ViewModel to the Model
            var answer = model.Adapt<Answer>();
            // override those properties
            //   that should be set from the server-side only
            answer.QuestionId = model.QuestionId;
            answer.Text = model.Text;
            answer.Notes = model.Notes;
            // properties set from server-side
            answer.CreatedDate = DateTime.Now;
            answer.LastModifiedDate = answer.CreatedDate;
            // add the new answer
            DbContext.Answers.Add(answer);
            // persist the changes into the Database.
            await DbContext.SaveChangesAsync();
            
            // return the newly-created Answer to the client.
            return new JsonResult(answer.Adapt<AnswerViewModel>(), JsonSettings);
        }
        
        /// <summary>
        /// Edit the Answer with the given {id}
        /// </summary>
        /// <param name="model">The AnswerViewModel containing the data to update</param>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]AnswerViewModel model)
        {
            // return a generic HTTP Status 500 (Server Error)
            // if the client payload is invalid.
            if (model == null) return new StatusCodeResult(500);
            // retrieve the answer to edit
            var answer = await DbContext.Answers.FirstOrDefaultAsync(q => q.Id == model.Id);
            // handle requests asking for non-existing answers
            if (answer == null)
            {
                return NotFound(new
                {
                    Error = String.Format("Answer ID {0} has not been found", model.Id)
                });
            }
            // handle the update (without object-mapping)
            //   by manually assigning the properties
            //   we want to accept from the request
            answer.QuestionId = model.QuestionId;
            answer.Text = model.Text;
            answer.Value = model.Value;
            answer.Notes = model.Notes;
            // properties set from server-side
            answer.LastModifiedDate = answer.CreatedDate;
            // persist the changes into the Database.
            await DbContext.SaveChangesAsync();
            
            // return the updated Quiz to the client.
            return new JsonResult(answer.Adapt<AnswerViewModel>(), JsonSettings);
        }
        /// <summary>
        /// Deletes the Answer with the given {id} from the Database
        /// </summary>
        /// <param name="id">The ID of an existing Answer</param>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            // retrieve the answer from the Database
            var answer = await DbContext.Answers
                .FirstOrDefaultAsync(i => i.Id == id);
            // handle requests asking for non-existing answers
            if (answer == null)
            {
                return NotFound(new
                {
                    Error = String.Format("Answer ID {0} has not been found", id)
                }); }
            // remove the quiz from the DbContext.
            DbContext.Answers.Remove(answer);
            // persist the changes into the Database.
            await DbContext.SaveChangesAsync();
            // return an HTTP Status 200 (OK).
            return new OkResult();
        }
        #endregion


        // GET api/answer/all
        [HttpGet("All/{questionId}")]
        public IActionResult All(int questionId)
        {
            var answers = DbContext.Answers
                .Where(q => q.QuestionId == questionId)
                .ToArray();
            return new JsonResult(
                answers.Adapt<AnswerViewModel[]>(),JsonSettings);
        }
    }
}
