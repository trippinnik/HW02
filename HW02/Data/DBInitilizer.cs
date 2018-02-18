using HW02.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HW02.Data
{
    public class DBInitilizer
    {
        public static void Initialize(TaskDatabaseContext context)
        {
            //check to see if there is already data in table
            if (context.Tasks.Any())
            {
                //do nothing because there is stuff already there
                return;
            }
            // see some data to get this rollding
            Tasks[] tasks = new Tasks[]
                {
                    new Tasks(){TaskName = "Buy groceries", IsCompleted = false, DueDate = "2018-02-03"},
                    new Tasks(){TaskName = "Workout", IsCompleted = true, DueDate = "2018-01-01"},
                    new Tasks(){TaskName = "Paint fence", IsCompleted = false, DueDate = "2018-03-15"},
                    new Tasks(){TaskName = "Mow Lawn", IsCompleted = false, DueDate = "2018-06-11"}
                };

            //loop the array of objects and insert the data
            foreach (Tasks task in tasks)
            {
                context.Tasks.Add(task);
            }
            //commit to the db
            context.SaveChanges();

        }
    }
}
