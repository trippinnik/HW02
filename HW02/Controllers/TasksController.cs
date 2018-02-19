using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using HW02.Common;
using HW02.CustomSettings;
using HW02.Data;
using HW02.DataTransferObjects;
using HW02.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HW02.Controllers
{
    /// <summary>
    /// provides the implementation for the Tasks resource
    /// </summary>
    [Produces("application/json")]
    [Route("api/Tasks")]
    public class TasksController : Controller
    {
        /// <summary>
        /// get tasks by id
        /// </summary>
        private const string GetTasksByIdRoute = "GetCustomerByIdRoute";

        /// <summary>
        /// DB context
        /// </summary>
        private readonly TaskDatabaseContext _context;

        /// <summary>
        /// logger instance
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// The configuration instance
        /// </summary>
        private readonly IDesignTimeMvcBuilderConfiguration _configuration;

        /// <summary>
        /// the task limits settings
        /// </summary>
        private readonly TasksLimits _tasksLimits;

        ///<summary>
        ///Inititilize instance of TasksController
        ///</summary>
        public TasksController(ILogger<TasksController> logger,
                                TaskDatabaseContext context,
                                IDesignTimeMvcBuilderConfiguration configuration,
                                IOptions<TasksLimits> tasksLimits)
        {
            _logger = logger;
            _context = context;
            _configuration = configuration;
            _tasksLimits = tasksLimits.Value;
        }

        /// <summary>
        /// Gets Task by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>the task resource</returns>
        [HttpGet]
        [ProducesResponseType(typeof(Tasks), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(void), 500)]
        [Route("api/v1/tasks/{id}", Name = GetTasksByIdRoute)]
        public IActionResult GetTasksById(int id)
        {
            try
            {
                Tasks task = (from c in _context.Tasks where c.Id == id select c).SingleOrDefault();
                if (task == null)
                {
                    _logger.LogInformation(LoggingEvents.GetItem, $"TasksController Tasks(id=[{id}]) was not found.", id);
                }
                return new ObjectResult(task);
            }
            catch (Exception ex)
            {
                _logger.LogError(LoggingEvents.InternalError, ex, $"TasksController Get Tasks(id=[{id}] caused an error.", id);
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        /// Gets the list of tasks
        /// </summary>
        /// <returns>The list of tasks</returns>
        [HttpGet]
        [ProducesResponseType(typeof(int[]), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(void), 500)]
        [Route("api/v1/tasks")]
        public IActionResult GetAllTasks()
        {
            List<int?> tasksIds = (from c in _context.Tasks select c.Id).ToList();

            return new ObjectResult(tasksIds);
        }


        /// <summary>
        /// Updates or creates a task
        /// </summary>
        /// <param name="id"></param>
        /// <param name="taskUpdatePayload"></param>
        /// <returns></returns>
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(Tasks), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(void), 500)]
        [Route("api/v1/customers/{id}")]
        [HttpPut]
        public IActionResult UpdateOrCreateTasks(int id, [FromBody] TasksUpdatePayload tasksUpdatePayload)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Tasks taskEntity = (from c in _context.Tasks where c.Id == id select c).SingleOrDefault();

                    //if not found create new entity using the id specified
                    if (taskEntity == null)
                    {
                        using (IDbContextTransaction transaction = _context.Database.BeginTransaction())
                        {
                            //check out task limits
                            if (!CanAddMoreTasks())
                            {
                                return StatusCode((int)HttpStatusCode.Forbidden, "Task limit already reached");
                            }

                            taskEntity = new Tasks()
                            {
                                Id = id,
                                TaskName = tasksUpdatePayload.Name,
                                IsCompleted = tasksUpdatePayload.IsCompleted,
                                DueDate = tasksUpdatePayload.DueDate
                            };

                            // turn identity insert on so that we can set ID provided by the client
                            _context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT Tasks ON;");
                            _context.Tasks.Add(taskEntity);
                            _context.SaveChanges();

                            //turn id insert off so default bahavior for new ids will be used
                            _context.Database.ExecuteSqlCommand("SET IDENTITy_INSERT Tasks OFF");
                            transaction.Commit();

                            return CreatedAtRoute(GetTasksByIdRoute, new { id = taskEntity.Id }, taskEntity);
                        }
                    }
                    //update the entity specified by the caller
                    taskEntity.TaskName = tasksUpdatePayload.Name;
                    taskEntity.IsCompleted = tasksUpdatePayload.IsCompleted;
                    taskEntity.DueDate = tasksUpdatePayload.DueDate;

                    _context.SaveChanges();


                }
                else
                {
                    return BadRequest(ModelState);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(LoggingEvents.InternalError, ex, $"TasksController Put Tasks(id=[{id}]) caused an internal error.", id);
            }
            return NoContent();
        }

        /// <summary>
        /// Determines whether tasks can be added.
        /// </summary>
        /// <returns>
        ///   true if more tasks can be added false if not
        /// </returns>
        private bool CanAddMoreTasks()
        {
            long totalTasks = (from c in _context.Tasks select c).Count();


            if (_tasksLimits.MaxTasks > totalTasks)
            {
                return true;
            }

            return false;
        }



        /// <summary>
        /// Deletes the task
        /// </summary>
        /// <param name="id">The identifier of the tasks.</param>        
        /// <returns>An IAction result indicating HTTP 204 no content if success otherwise BadRequest if the input is not valid.</returns>
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(void), 500)]
        [Route("api/v1/tasks/{id}")]
        [HttpDelete]
        public IActionResult DeleteTaskById(int id)
        {
            try
            {
                Tasks dbTask = (from c in _context.Tasks where c.Id == id select c).SingleOrDefault();

                if (dbTask == null)
                {
                    _logger.LogInformation(LoggingEvents.UpdateItem, $"TasksController Tasks(id=[{id}]) was not found.", id);
                    return NotFound();
                }

                _context.Tasks.Remove(dbTask);

                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.LogError(LoggingEvents.InternalError, ex, $"TasksController Put Tasks(id=[{id}]) caused an internal error.", id);
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }

            return NoContent();
        }

        /// <summary>
        /// Creates the task
        /// </summary>
        /// <param name="taskCreatePayload">The customer.</param>
        /// <returns>An IAction result indicating HTTP 201 created if success otherwise BadRequest if the input is not valid.</returns>
        [ProducesResponseType(typeof(Tasks), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(void), 500)]
        [Route("api/v1/tasks")]
        [HttpPost]
        public IActionResult CreateTask([FromBody] TasksCreatePayload taskCreatePayload)
        {
            Tasks taskEntity = new Tasks();

            try
            {
                if (ModelState.IsValid)
                {
                    // First verify that there are not the max customers allready
                    if (!CanAddMoreTasks())
                    {
                        return StatusCode((int)HttpStatusCode.Forbidden, "Task limit reached!!");
                    }

                    taskEntity.TaskName = taskCreatePayload.Name;
                    taskEntity.IsCompleted = taskCreatePayload.IsCompleted;
                    taskEntity.DueDate = taskCreatePayload.DueDate;


                    // Tell entity framework to add the address entity
                    _context.Tasks.Add(taskEntity);

                    _context.SaveChanges();
                }
                else
                {
                    return BadRequest(ModelState);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(LoggingEvents.InternalError, ex, $"TaskController Post TaskEntity([{taskEntity}]) TasksCreatePayload([{taskCreatePayload}] caused an internal error.");
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }

            return CreatedAtRoute(GetTasksByIdRoute, new { id = taskEntity.Id }, taskEntity);
        }

    }
}