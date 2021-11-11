using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unify.AzureFunctionAppTools.ExceptionHandling;

namespace Unify.AzureFunctionAppTools
{
    /// <summary>
    /// Base for handling requests.
    /// </summary>
    /// <typeparam name="TContext">Function request context.</typeparam>
    public abstract class RequestHandlerBaseBase<TContext>
        where TContext : FunctionRequestContext
    {
        private readonly bool _LogUncaughtErrors;

        /// <summary>
        /// Creates an instance of the request handler base.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <param name="logUncaughtErrors">If uncaught errors should be logged.</param>
        protected RequestHandlerBaseBase(IServiceProvider serviceProvider, bool logUncaughtErrors)
        {
            UnhandledErrorFactory = serviceProvider.GetRequiredService<IUnhandledErrorFactory>();
            _LogUncaughtErrors = logUncaughtErrors;
        }

        /// <summary>
        /// Factory for created unhandled error reponses.
        /// </summary>
        protected IUnhandledErrorFactory UnhandledErrorFactory { get; }

        /// <summary>
        /// Handles the request.
        /// </summary>
        /// <param name="context">Incoming context.</param>
        /// <returns>Action result to return.</returns>
        public abstract Task<IActionResult> Handle(TContext context);

        /// <summary>
        /// Handle the request.
        /// </summary>
        /// <param name="context">The request context.</param>
        /// <returns>The action result response to the request.</returns>
        public abstract Task<IActionResult> HandleRequest(TContext context);

        /// <summary>
        /// Handle errors during request validation..
        /// </summary>
        /// <param name="context">The request context.</param>
        /// <returns>The action result response to the error.</returns>
        public abstract Task<IActionResult> HandleValidationError(TContext context);

        /// <summary>
        /// Handle failure of request validation.
        /// </summary>
        /// <param name="context">The request context.</param>
        /// <returns>The action result response to the failure.</returns>
        public abstract Task<IActionResult> HandleValidationFailure(TContext context);

        protected internal async Task<IActionResult> HandleRequestSafely(TContext context)
        {
            try
            {
                return await HandleRequest(context);
            }
            catch (Exception e)
            {
                LogUnhandledFor(context, e, nameof(HandleRequest));
                return UnhandledErrorFactory.Create(e, additionalData: new Dictionary<string, object>());
            }
        }

        protected internal async Task<IActionResult> HandleValidationErrorSafely(TContext context)
        {
            try
            {
                return await HandleValidationError(context);
            }
            catch (Exception e)
            {
                LogUnhandledFor(context, e, nameof(HandleValidationError));
                return UnhandledErrorFactory.Create(e, additionalData: new Dictionary<string, object>());
            }
        }

        protected internal async Task<IActionResult> HandleValidationFailureSafely(TContext context)
        {
            try
            {
                return await HandleValidationFailure(context);
            }
            catch (Exception e)
            {
                LogUnhandledFor(context, e, nameof(HandleValidationError));
                return UnhandledErrorFactory.Create(e, additionalData: new Dictionary<string, object>());
            }
        }

        protected internal void LogUnhandledFor(TContext context, Exception e, string location)
        {
            if (_LogUncaughtErrors)
                context.Logger.LogWarning(e, string.Format(ToolResources.UnhandledErrorWarningMessage, location));
        }
    }
}
