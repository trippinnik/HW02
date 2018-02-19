using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HW02.DataTransferObjects
{
    /// <summary>
    /// tasks creation payload
    /// </summary>
    public class TasksCreatePayload
    {
        /// <summary>
        /// gets or sets the task name
        /// </summary>
        /// <value>name
        /// </value>
        [Required]
        [StringLength(100)]
        public string TaskName { get; set; }
        
        /// <summary>
        /// sets completed status
        /// </summary>
        /// <value>
        /// true or false
        /// </value>
        [Required]
        public bool IsCompleted { get; set; }

        /// <summary>
        /// gets or sets the due date
        /// </summary>
        /// <value>
        /// string holding date yyyy-MM-dd
        /// </value>
        [Required]
        [StringLength(10)]
        public string DueDate { get; set; }
        /// <summary>
        /// return a <see cref="System.String"/> that represents this instance
        /// </summary>
        /// <returns>return a <see cref="System.String"/> that represents this instance</returns>
        public override string ToString()
        {
            return $"Name=[{TaskName}], IsCompleted=[{IsCompleted}], DueDate=[{DueDate}]";
        }

    }
}
