using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Unify.AzureFunctionAppTools
{
    /// <summary>
    /// Base for request handlers.
    /// </summary>
    public abstract class RequestHandlerBase<TBody> : RequestHandlerBaseBase<FunctionRequestContext<TBody>>
    {
        /// <summary>
        /// Creates an instance of the request handler base.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <param name="logUnhandledErrors">If unhandled errors should be logged.</param>
        protected RequestHandlerBase(IServiceProvider serviceProvider, bool logUnhandledErrors = true) 
            : base(serviceProvider, logUnhandledErrors)
        {
        }

        /// <inheritdoc />
        public override async Task<IActionResult> Handle(FunctionRequestContext<TBody> context)
        {
            return context.Status switch
            {
                RequestContextStatus.Success => await HandleRequestSafely(context),
                RequestContextStatus.BodyReadError => await HandleBodyReadErrorSafely(context),
                RequestContextStatus.BodyDeserializationError => await HandleBodyDeserializationErrorSafely(context),
                RequestContextStatus.ValidationError => await HandleValidationErrorSafely(context),
                RequestContextStatus.ValidationFailure => await HandleValidationFailureSafely(context),
                RequestContextStatus.PreprocessorHalt => context.PreprocessorHaltResponse,
                _ => UnhandledErrorFactory.Create("Unexpected context status.", additionalData: new Dictionary<string, object>
                {
                    ["statusValue"] = (int) context.Status
                })
            };
        }

        /// <summary>
        /// Handle errors during request body reading.
        /// </summary>
        /// <param name="context">The request context.</param>
        /// <returns>The action result response to the error.</returns>
        public abstract Task<IActionResult> HandleBodyReadError(FunctionRequestContext<TBody> context);

        /// <summary>
        /// Handle errors during request body deserialization.
        /// </summary>
        /// <param name="context">The request context.</param>
        /// <returns>The action result response to the error.</returns>
        public abstract Task<IActionResult> HandleBodyDeserializationError(FunctionRequestContext<TBody> context);

        private async Task<IActionResult> HandleBodyReadErrorSafely(FunctionRequestContext<TBody> context)
        {
            try
            {
                return await HandleBodyReadError(context);
            }
            catch (Exception e)
            {
                LogUnhandledFor(context, e, nameof(HandleBodyReadError));
                return UnhandledErrorFactory.Create(e, additionalData: new Dictionary<string, object>());
            }
        }

        private async Task<IActionResult> HandleBodyDeserializationErrorSafely(FunctionRequestContext<TBody> context)
        {
            try
            {
                return await HandleBodyDeserializationError(context);
            }
            catch (Exception e)
            {
                LogUnhandledFor(context, e, nameof(HandleBodyDeserializationError));
                return UnhandledErrorFactory.Create(e, additionalData: new Dictionary<string, object>());
            }
        }
    }
}
