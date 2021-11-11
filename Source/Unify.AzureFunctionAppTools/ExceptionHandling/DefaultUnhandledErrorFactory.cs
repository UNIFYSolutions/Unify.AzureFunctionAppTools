using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Unify.AzureFunctionAppTools.ExceptionHandling
{
    /// <summary>
    /// The default factory for creating internal server error responses from errors or exceptions. 
    /// </summary>
    public class DefaultUnhandledErrorFactory : IUnhandledErrorFactory
    {
        private readonly ILogger _Log;
        private readonly bool _IncludeExceptions;

        /// <summary>
        /// Constructor for this factory.
        /// </summary>
        /// <param name="log">The log to write to.</param>
        /// <param name="includeExceptions">If exception details should be included in the response.</param>
        public DefaultUnhandledErrorFactory(ILogger log, bool includeExceptions)
        {
            _Log = log;
            _IncludeExceptions = includeExceptions;
        }

        /// <inheritdoc />
        public IActionResult Create(string message, IDictionary<string, object> additionalData = null) => Create(message, null, additionalData);

        /// <inheritdoc />
        public IActionResult Create(Exception e, IDictionary<string, object> additionalData = null) => Create("Internal server error", e, additionalData);

        /// <inheritdoc />
        public IActionResult Create(string message, Exception e, IDictionary<string, object> additionalData = null)
        {
            _Log.LogError(e != null ? $"{message} - Exception:{Environment.NewLine}{e}" : message);

            var response = new DefaultErrorResponseBody { Message = message, AdditionalData = additionalData };

            if (e != null && _IncludeExceptions) response.Exception = e.ToString();

            return new ObjectResult(response) { StatusCode = 500 };
        }
    }
}
