using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace HW02.Models
{
    /// <summary>
    /// the task entity
    /// </summary>
    [CustomValidation(typeof(Tasks), "ValidateNameCompletedDate")]
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
    
    /// <summary>
    /// Validates the Task, Completed Status and Date
    /// </summary>
    /// <param name="task">the task object</param>
    /// <param name="context">value of the field that the validator is validating</param>
    /// <returns></returns>
    public static ValidationResult ValidateNameCompletedDate(Tasks task)
    {
        //verify task name is not null or empty
     if (task.TaskName == null || task.TaskName == "")
     {
         return new ValidationResult("Missing Task name.", new List<string> { "TaskName" });
     }
        //check if taskname is longer than 100 characters
     if (task.TaskName.Length > 100)
     {
           return new ValidationResult("Task name is too long.", new List<string> { "TaskName" });
     }
        //check if Unique value of TaskName 
        //something will be needed to get the values of the TaskNames already in DB
        //bool will never be null how to check ?
     if (task.DueDate == null)
     {
           return new ValidationResult("Date is missing", new List<string> { "DueDate" });
     }
        //check if the data is the correct format
     DateTime Test;
     if (!DateTime.TryParseExact(task.DueDate, "yyyy-MM-dd", new CultureInfo("en-US"), DateTimeStyles.None, out Test))
     {
           return new ValidationResult("Date is not in format 'yyyy-MM-dd'", new List<string> { "DueDate" });
     }

       return ValidationResult.Success;
    }
        /// <summary>
        /// check the taskname is not null
        /// </summary>
        /// <returns>bool</returns>
        public bool IsNotNullTaskName(String taskName)
        {
            //verify task name is not null or empty
            if (taskName == null || taskName == "")
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// validation that the task name is not greater than 100
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public bool IsValidTaskName(string taskName)
        {
            //check if taskname is longer than 100 characters
            if (taskName.Length > 100)
            {
                return false;
            }
            return true;
        }
        
        /// <summary>
        /// Check that due date is not null or empty
        /// </summary>
        /// <param name="dueDate"></param>
        /// <returns></returns>
        public bool IsNotNullDueDate(String dueDate)
        {
            {
                //verify task name is not null or empty
                if (dueDate == null || dueDate == "")
                {
                    return false;
                }
                return true;
            }
        }
        /// <summary>
        /// check that the date format matches yyyy-MM-dd
        /// </summary>
        /// <param name="dueDate"></param>
        /// <returns></returns>
        public bool IsValidDueDate(String dueDate)
        {
            DateTime Test;
            if (!DateTime.TryParseExact(dueDate, "yyyy-MM-dd", new CultureInfo("en-US"), DateTimeStyles.None, out Test))
            {
                return false;
            }
            return true;
        }
        
    }
    
    
   
}
