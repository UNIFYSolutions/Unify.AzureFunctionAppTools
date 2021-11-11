using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Unify.AzureFunctionAppTools
{
    /// <summary>
    /// Context of a function request.
    /// </summary>
    public class FunctionRequestContext
    {
        /// <summary>
        /// The status of the context.
        /// </summary>
        public RequestContextStatus Status { get; init; }

        /// <summary>
        /// The request this context encapsulates.
        /// </summary>
        public HttpRequest Request { get; init; }

        /// <summary>
        /// The functions logger.
        /// </summary>
        public ILogger Logger { get; set; }

        /// <summary>
        /// The headers of the request.
        /// </summary>
        public IDictionary<string, string[]> Headers { get; init; }

        /// <summary>
        /// The query parameters of the request.
        /// </summary>
        public IDictionary<string, string[]> QueryParameter { get; init; }

        /// <summary>
        /// Results of the header validation.
        /// </summary>
        public RequestValidationResult HeaderValidationResult { get; init; }

        /// <summary>
        /// Results of the query parameter validation.
        /// </summary>
        public RequestValidationResult QueryParameterValidationResult { get; init; }

        /// <summary>
        /// An exception that was raised during the creation of the context.
        /// </summary>
        public Exception Exception { get; init; }

        /// <summary>
        /// Additional data related to the request.
        /// </summary>
        public Dictionary<string, object> RequestMetadata { get; init; } = new Dictionary<string, object>();

        /// <summary>
        /// A action result to return if a preprocessor has indicated the request should be halted.
        /// </summary>
        public IActionResult PreprocessorHaltResponse { get; init; }
    }
}
