using System.Collections.Generic;

namespace Unify.AzureFunctionAppTools.ExceptionHandling
{
    /// <summary>
    /// Default error response body.
    /// </summary>
    public class DefaultErrorResponseBody
    {
        /// <summary>
        /// Error message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// The error exception.
        /// </summary>
        public string Exception { get; set; }

        /// <summary>
        /// Additional details about the exception.
        /// </summary>
        public IDictionary<string, object> AdditionalData { get; set; }
    }
}
