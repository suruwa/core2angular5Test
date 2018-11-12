using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace core2angular5test.Data.Models
{
    public class Token
    {
        #region constructor

        public Token()
        {
            
        }
        #endregion
        
        #region properties
        [Key]
        [Required]
        public int Id { get; set; }
        
        [Required]
        public string ClientId { get; set; }
        [Required]
        public string Value { get; set; }
        
        /// <summary>
        /// Zero for refresh tokens
        /// </summary>
        public int Type { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        public DateTime CreatedDate { get; set; }        
        #endregion
        
        #region Lazy-Load properties
        /// <summary>
        /// The user related to this token
        /// </summary>
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }
        #endregion
    }
}