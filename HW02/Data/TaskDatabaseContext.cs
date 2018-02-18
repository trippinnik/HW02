using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HW02.Models;
using Microsoft.EntityFrameworkCore;

namespace HW02.Data
{
    /// <summary>
    /// Coordinate our Tasks model with Entity framework
    /// </summary>
    public class TaskDatabaseContext : DbContext
    {
        /// <summary>
        /// Inititialize new instance of <see cref="TaskDatabaseContext"/> class
        /// </summary>
        /// <param name="options"></param>
        public TaskDatabaseContext(DbContextOptions<TaskDatabaseContext> options) : base(options)
        {
            //create db and table
            Database.EnsureCreated();
        }
        /// <summary>
        /// Represents the Tasks table
        /// </summary>
        /// <value>
        /// The tasks
        /// </value>
        public DbSet<Tasks> Tasks { get; set; }
        /// <summary>
        /// override to provide the mapping of Tasks model to the database table
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //link the Tasks model to the Tasks table
            modelBuilder.Entity<Tasks>().ToTable("Tasks");
        }
    }
}
