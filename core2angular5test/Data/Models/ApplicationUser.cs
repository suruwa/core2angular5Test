using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace core2angular5test.Data.Models
{
    public class ApplicationUser
    {
        #region constructor
        public ApplicationUser()
        {
            
        }
        #endregion
        
        #region properties
        [Key]
        [Required]
        public string Id { get; set; }
        
        [Required]
        [MaxLength(128)]
        public string UserName { get; set; }
        
        [Required]
        public string Email { get; set; }
        
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