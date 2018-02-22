using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace HW02.DataTransferObjects
{   
    /// <summary>
    /// Provide payload to sort all tasks and retrieve
    /// </summary>
    public class GetAllTasksPayload
    {
        /// <summary>
        /// Order the Tasks in Ascending or Descending
        /// </summary>
        /// <value>Asc or Desc</value>
        /// <example>Desc</example>

        [DefaultValue("Asc")]
        public String OrderByDate { get; set; }
        /// <summary>
        /// Filter TaskStatus Completed or NotCompleted
        /// </summary>
        /// <value>Completed or NotCompleted</value>
        /// <example>Completed</example>
        [DefaultValue("All")]
        public String TaskStatus { get; set; }

        /// <summary>
        /// Check if OrderByDate is valid
        /// </summary>
        /// <returns>bool</returns>
        public bool IsValidOrderByDate()
        {
            if(OrderByDate == "Asc" || OrderByDate == "Desc")
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// Check if TaskStatus is Valid
        /// </summary>
        /// <returns>bool</returns>
        public bool IsValidTaskStatus()
        {
            if(TaskStatus == "Completed" || TaskStatus == "NotCompleted" || TaskStatus == "All")
            {
                return true;
            }
            return false;
        }
    }
}
