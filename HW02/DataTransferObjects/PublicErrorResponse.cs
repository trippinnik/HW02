using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HW02.DataTransferObjects
{
    /// <summary>
    /// provide object for error responses
    /// </summary>
    public class PublicErrorResponse
    {
        /// <summary>
        /// error number code
        /// </summary>
        public int errorNumber { get; set; }
        /// <summary>
        /// the name of the paramter that caused the error
        /// </summary>
        public string parameterName { get; set; }

        /// <summary>
        /// the value of the paramater that caused the error
        /// </summary>
        public string parameterValue { get; set; }

        /// <summary>
        /// Error Description
        /// </summary>
        public string errorDescription { get; set; }

    }
}
