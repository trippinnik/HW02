using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HW02.CustomSettings;
using HW02.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

     
    }
}