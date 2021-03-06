﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HW02.Controllers
{
    /// <summary>
    /// provides the implementation for the Tasks resource
    /// </summary>
    [Produces("application/json")]
    public class TasksController : Controller
    {
        /// <summary>
        /// get tasks by id
        /// </summary>
        private const string GetTasksByIdRoute = "GetTasksByIdRoute";

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
        private readonly IConfiguration _configuration;

        /// <summary>
        /// the task limits settings
        /// </summary>
        private readonly TasksLimits _tasksLimits;

        ///<summary>
        ///Inititilize instance of TasksController
        ///</summary>
        public TasksController(ILogger<TasksController> logger,
                                TaskDatabaseContext context,
                                IConfiguration configuration,
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
                    PublicErrorResponse publicErrorResponse = new PublicErrorResponse()
                    {
                        errorNumber = 5,
                        parameterName = "id",
                        parameterValue = id.ToString(),
                        errorDescription = "The entity could not be found"
                    };

                    return StatusCode((int)HttpStatusCode.NotFound, publicErrorResponse);
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
        public IActionResult GetAllTasks(GetAllTasksPayload getAllTasksPayload)
        {
            List<int?> tasksIds = (from c in _context.Tasks select c.Id).ToList();

            return new ObjectResult(tasksIds);
        }


        /// <summary>
        /// Updates or creates a task
        /// </summary>
        /// <param name="id"></param>
        /// <param name="tasksUpdatePayload"></param>
        /// <returns></returns>
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(Tasks), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(void), 500)]
        [Route("api/v1/tasks/{id}")]
        [HttpPut]
        public IActionResult UpdateTasks(int id, [FromBody] TasksUpdatePayload tasksUpdatePayload)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    
                    Tasks taskEntity = (from c in _context.Tasks where c.Id == id select c).SingleOrDefault();
                    Tasks taskTest = new Tasks();
                    //check if the taskname exhists and that taskname is not the record we are updating
                    Tasks checkTaskNameDuplicate = (from r in _context.Tasks where r.TaskName == tasksUpdatePayload.TaskName && id != r.Id select r).SingleOrDefault();
                    if (!(checkTaskNameDuplicate == null))
                    {
                        PublicErrorResponse publicErrorResponse = new PublicErrorResponse() { errorNumber = 1, parameterName = "taskName", parameterValue = taskEntity.TaskName, errorDescription = "The Task Name already exhists." };

                        return StatusCode((int)HttpStatusCode.Conflict, new BadRequestObjectResult(publicErrorResponse));
                    } 
                    if (!taskTest.IsNotNullTaskName(tasksUpdatePayload.TaskName))
                    {
                        PublicErrorResponse publicErrorResponse = new PublicErrorResponse() { errorNumber = 2, parameterName = "taskName", parameterValue = taskEntity.TaskName, errorDescription = "The Task Name is missing." };

                        return StatusCode((int)HttpStatusCode.Conflict, new BadRequestObjectResult(publicErrorResponse));
                    }
                    if (!taskTest.IsValidTaskName(tasksUpdatePayload.TaskName))
                    {
                        PublicErrorResponse publicErrorResponse = new PublicErrorResponse() { errorNumber = 2, parameterName = "taskName", parameterValue = taskEntity.TaskName, errorDescription = "The Task Name is invalid." };

                        return StatusCode((int)HttpStatusCode.Conflict, new BadRequestObjectResult(publicErrorResponse));
                    }
                    if (!taskTest.IsNotNullDueDate(tasksUpdatePayload.DueDate))
                    {
                        PublicErrorResponse publicErrorResponse = new PublicErrorResponse() { errorNumber = 2, parameterName = "dueDate", parameterValue = taskEntity.TaskName, errorDescription = "The due date is missing." };

                        return StatusCode((int)HttpStatusCode.Conflict, new BadRequestObjectResult(publicErrorResponse));
                    }
                    if (!taskTest.IsValidDueDate(tasksUpdatePayload.DueDate))
                    {
                        PublicErrorResponse publicErrorResponse = new PublicErrorResponse() { errorNumber = 2, parameterName = "dueDate", parameterValue = taskEntity.TaskName, errorDescription = "The due date is invalid, the date needs to be in formate yyyy-MM-dd." };

                        return StatusCode((int)HttpStatusCode.Conflict, new BadRequestObjectResult(publicErrorResponse));
                    }
                    //if not found create new entity using the id specified
                    if (taskEntity == null)
                    {

                        PublicErrorResponse publicErrorResponse = new PublicErrorResponse() {
                            errorNumber = 5,
                            parameterName = "id",
                            parameterValue = id.ToString(),
                            errorDescription = "The entity could not be found"
                        };

                    return StatusCode((int)HttpStatusCode.Forbidden, publicErrorResponse);              
                                                    
                    }
                    
                    
                    //update the entity specified by the caller
                    taskEntity.TaskName = tasksUpdatePayload.TaskName;
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
        /// Find out if the task name is already in use
        /// </summary>
        /// <param name="taskName">String that should be the taskname to check for exhistance</param>
        /// <returns></returns>
        private bool TaskNameIsUnique(String taskName)
        {
            
                var dbTask = (from c in _context.Tasks where c.TaskName == taskName select c).SingleOrDefault();
                if (dbTask == null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
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
                    PublicErrorResponse publicErrorResponse = new PublicErrorResponse() { 
                                                                                          errorNumber = 5, 
                                                                                          parameterName = "id",
                                                                                          parameterValue = id.ToString(),
                                                                                          errorDescription = "The entity could not be found"
                                                                                        };
                
                    return StatusCode((int)HttpStatusCode.NotFound, publicErrorResponse);
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
        /// <param name="taskCreatePayload">The task.</param>
        /// <returns>An IAction result indicating HTTP 201 created if success otherwise BadRequest if the input is not valid.</returns>
        [ProducesResponseType(typeof(Tasks), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
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
                    // First verify that there are not the max tasks already
                    if (!CanAddMoreTasks())
                    {
                        PublicErrorResponse publicErrorResponse = new PublicErrorResponse() { errorNumber = 4, parameterName = null, parameterValue = null, errorDescription = "The maximum number of entities have been created. No further entities can be created at this time." };
                        return StatusCode((int)HttpStatusCode.Forbidden, publicErrorResponse);
                    }
                    if (!TaskNameIsUnique(taskCreatePayload.TaskName))
                    {
                        PublicErrorResponse publicErrorResponse = new PublicErrorResponse() { errorNumber = 1, parameterName = "taskName", parameterValue = taskEntity.TaskName, errorDescription = "The Task Name already exhists." };

                        return StatusCode((int)HttpStatusCode.Conflict, new BadRequestObjectResult(publicErrorResponse));
                    }

                    if (!taskEntity.IsNotNullTaskName(taskCreatePayload.TaskName))
                    {
                        PublicErrorResponse publicErrorResponse = new PublicErrorResponse() { errorNumber = 2, parameterName = "taskName", parameterValue = taskEntity.TaskName, errorDescription = "The Task Name is missing." };

                        return StatusCode((int)HttpStatusCode.Conflict, new BadRequestObjectResult(publicErrorResponse));
                    }
                    if (!taskEntity.IsValidTaskName(taskCreatePayload.TaskName))
                    {
                        PublicErrorResponse publicErrorResponse = new PublicErrorResponse() { errorNumber = 2, parameterName = "taskName", parameterValue = taskEntity.TaskName, errorDescription = "The Task Name is invalid." };

                        return StatusCode((int)HttpStatusCode.Conflict, new BadRequestObjectResult(publicErrorResponse));
                    }
                    if (!taskEntity.IsNotNullDueDate(taskCreatePayload.DueDate))
                    {
                        PublicErrorResponse publicErrorResponse = new PublicErrorResponse() { errorNumber = 2, parameterName = "dueDate", parameterValue = taskEntity.TaskName, errorDescription = "The due date is missing." };

                        return StatusCode((int)HttpStatusCode.Conflict, new BadRequestObjectResult(publicErrorResponse));
                    }
                    if (!taskEntity.IsValidDueDate(taskCreatePayload.DueDate))
                    {
                        PublicErrorResponse publicErrorResponse = new PublicErrorResponse() { errorNumber = 2, parameterName = "dueDate", parameterValue = taskEntity.TaskName, errorDescription = "The due date is invalid, the date needs to be in formate yyyy-MM-dd." };

                        return StatusCode((int)HttpStatusCode.Conflict, new BadRequestObjectResult(publicErrorResponse));
                    }

                    taskEntity.TaskName = taskCreatePayload.TaskName;
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