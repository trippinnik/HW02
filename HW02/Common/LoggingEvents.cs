﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HW02.Common
{
    public class LoggingEvents
    {
        /// <summary>
        /// The get item
        /// </summary>
        public const int GetItem = 1000;
        /// <summary>
        /// The insert item
        /// </summary>
        public const int InsertItem = 1001;
        /// <summary>
        /// The update item
        /// </summary>
        public const int UpdateItem = 1002;
        /// <summary>
        /// The delete item
        /// </summary>
        public const int DeleteItem = 1003;

        /// <summary>
        /// The get item not found
        /// </summary>
        public const int GetItemNotFound = 4000;
        /// <summary>
        /// The update item not found
        /// </summary>
        public const int UpdateItemNotFound = 4001;
        /// <summary>
        /// The delete item not found
        /// </summary>
        public const int DeleteItemNotFound = 4002;
        /// <summary>
        /// The internal error
        /// </summary>
        public const int InternalError = 5000;
    }
}
