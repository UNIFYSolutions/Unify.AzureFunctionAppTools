using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace Unify.AzureFunctionAppTools.ExceptionHandling
{
    /// <summary>
    /// A factory for creating internal server error respones from unhandled errors or exceptions.
    /// </summary>
    public interface IUnhandledErrorFactory
    {
        /// <summary>
        /// Create an internal server error response.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="additionalData">Any additional error data to be included.</param>
        /// <returns>The 500 action result.</returns>
        public IActionResult Create(string message, IDictionary<string, object> additionalData = null);

        /// <summary>
        /// Create an internal server error response.
        /// </summary>
        /// <param name="e">The exception which caused this error.</param>
        /// <param name="additionalData">Any additional error data to be included.</param>
        /// <returns>The 500 action result.</returns>
        public IActionResult Create(Exception e, IDictionary<string, object> additionalData = null);

        /// <summary>
        /// Create an internal server error response.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="e">The exception which caused this error.</param>
        /// <param name="additionalData">Any additional error data to be included.</param>
        /// <returns>The 500 action result.</returns>
        public IActionResult Create(string message, Exception e, IDictionary<string, object> additionalData = null);
    }
}
