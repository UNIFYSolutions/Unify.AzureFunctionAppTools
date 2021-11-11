using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Unify.AzureFunctionAppTools
{
    /// <summary>
    /// Base for request handlers.
    /// </summary>
    public abstract class RequestHandlerBase : RequestHandlerBaseBase<FunctionRequestContext>
    {
        /// <summary>
        /// Creates an instance of the request handler base.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <param name="logUnhandledErrors">If uncaught errors should be logged.</param>
        public RequestHandlerBase(IServiceProvider serviceProvider, bool logUnhandledErrors = true) 
            : base(serviceProvider, logUnhandledErrors)
        {
        }

        /// <inheritdoc />
        public override async Task<IActionResult> Handle(FunctionRequestContext context)
        {
            return context.Status switch
            {
                RequestContextStatus.Success => await HandleRequestSafely(context),
                RequestContextStatus.ValidationError => await HandleValidationErrorSafely(context),
                RequestContextStatus.ValidationFailure => await HandleValidationFailureSafely(context),
                RequestContextStatus.PreprocessorHalt => context.PreprocessorHaltResponse,
                _ => UnhandledErrorFactory.Create("Unexpected context status.", additionalData: new Dictionary<string, object>
                {
                    ["statusValue"] = (int) context.Status
                })
            };
        }

        
    }
}
