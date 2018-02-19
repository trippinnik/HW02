using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HW02.DataTransferObjects
{
    /// <summary>
    /// Define the public facing tasks attributes
    /// </summary>
    public class TasksResult
    {
        /// <summary>
        /// gets or sets task id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// gets or sets Is Completed Status
        /// </summary>
        /// <value>
        /// true or false 
        /// </value>
        public bool IsCompleted { get; set; }

        /// <summary>
        /// gets or sets the due date
        /// </summary>
        /// <value>
        /// string with date yyyy-MM-dd
        /// </value>
        public string DueDate { get; set; }
    }
}
