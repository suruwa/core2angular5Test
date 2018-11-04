using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace core2angular5test.Data.Models
{
    public class ApplicationUser : IdentityUser
    {
        #region constructor
        public ApplicationUser()
        {
            
        }
        #endregion
        
        #region properties        
        
        public string Notes { get; set; }
        
        [Required]
        public int Type { get; set; }
        
        [Required]
        public int Flags { get; set; }
        
        [Required]
        public DateTime CreatedDate { get; set; }
        
        [Required]
        public DateTime LastModifiedDate { get; set; }
        #endregion
        
        #region Lazy-Load Properties
        /// <summary>
        /// A list of all the quiz created by this users.
        /// </summary>
        public virtual List<Quiz> Quizzes { get; set; }
        #endregion
    }
}