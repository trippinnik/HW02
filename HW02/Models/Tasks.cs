using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace HW02.Models
{
    /// <summary>
    /// the task entity
    /// </summary>
    [CustomValidation(typeof(Task), "ValidateNameCompletedDate")]
    public class Tasks
    {
        /// <summary>
        /// generate the key on db. we'll let this happen automagically
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? Id { get; set; }
        /// <summary>
        /// This is the name of the task
        /// It is required
        /// takes a string
        /// </summary>
        [Required]
        [StringLength(100)]
        public string TaskName { get; set; }
        /// <summary>
        /// flag if task is completed or not
        /// this is required
        /// </summary>
        [Required]
        public bool IsCompleted { get; set; }
        /// <summary>
        /// Due data of the task it is required
        /// takes type string
        /// </summary>
        [Required]
        public string DueDate { get; set; }
    }
}
