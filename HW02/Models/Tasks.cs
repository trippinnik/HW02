using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HW02.Models
{
    public class Tasks
    {
        /// <summary>
        /// This is the name of the task
        /// It is required
        /// takes a string
        /// </summary>
        public string TaskName { get; set; }
        /// <summary>
        /// flag if task is completed or not
        /// this is required
        /// </summary>
        public bool IsCompleted { get; set; }
        /// <summary>
        /// Due data of the task it is required
        /// takes type string
        /// </summary>
        public string DueDate { get; set; }
    }
}
