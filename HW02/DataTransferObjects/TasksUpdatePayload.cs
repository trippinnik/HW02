using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HW02.DataTransferObjects
{
    /// <summary>
    /// This is the payload to update tasks
    /// </summary>
    public class TasksUpdatePayload
    {
        /// <summary>
        /// gets or sets the task name
        /// </summary>
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        /// <summary>
        /// gets or sets IsCompleted flag
        /// </summary>
        /// <value>
        /// IsCompleted true or false
        /// </value>
        [Required]
        public bool IsCompleted { get; set; }

        /// <summary>
        /// gets or sets the due date
        /// </summary>
        /// <value>
        /// string with date in format yyyy-MM-dd
        /// </value>
        [Required]
        [StringLength(10)]
        public string DueDate { get; set; }
        
        /// <summary>
        /// returns <see cref="System.String"/> of property values in instance
        /// </summary>
        /// <returns>A <see cref="String"/> that represents this instance</returns>
        public override string ToString()
        {
            return $"Name=[{Name}], IsCompleted=[{IsCompleted}], DueDate=[{DueDate}]";
        }
    }
}
