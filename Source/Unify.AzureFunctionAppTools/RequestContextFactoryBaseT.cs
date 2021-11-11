using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Unify.AzureFunctionAppTools.ExceptionHandling;
using Unify.AzureFunctionAppTools.Preprocessing;

namespace Unify.AzureFunctionAppTools
{
    /// <summary>
    /// Base request context factory, for requests with a body.
    /// </summary>
    /// <typeparam name="TBody">Type of the request body.</typeparam>
    public abstract class RequestContextFactoryBase<TBody> : RequestContextFactoryBase, IRequestContextFactory<TBody> where TBody : class
    {
        /// <summary>
        /// Constructor for this factory.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        protected RequestContextFactoryBase(IServiceProvider serviceProvider) 
            : base(serviceProvider)
        {
        }

        /// <inheritdoc />
        public new async Task<FunctionRequestContext<TBody>> Create(HttpRequest request, ILogger logger)
        {
            // Get base reader result containing header and query param and their validation results.
            var baseResult = await InnerCreate(request, logger);

            if (baseResult.Status != RequestContextStatus.Success)
            {
                return new FunctionRequestContext<TBody>(baseResult)
                {
                    BodyValidationResult = RequestValidationResult.Unvalidated
                };
            }

            string rawRequestBody = null;
            TBody requestBody = null;
            RequestValidationResult bodyValidationResult = null;
            
            if (request.Body != null)
            {
                // Read body.
                try
                {
                    using var reader = new StreamReader(request.Body);
                    rawRequestBody = await reader.ReadToEndAsync();
                }
                catch (Exception e)
                {
                    logger.LogDebug(e, ToolResources.ContextCreateBodyReadErrorMessage);
                    return new FunctionRequestContext<TBody>(baseResult)
                    {
                        Status = RequestContextStatus.BodyReadError,
                        BodyValidationResult = RequestValidationResult.Unvalidated,
                        Exception = e,
                    };
                }
            
                // Parse body.
                try
                {
                    requestBody = JsonConvert.DeserializeObject<TBody>(rawRequestBody);
                }
                catch (Exception e)
                {
                    logger.LogDebug(e, ToolResources.ContextCreateBodyReadParseMessage);
                    return new FunctionRequestContext<TBody>(baseResult)
                    {
                        Status = RequestContextStatus.BodyDeserializationError,
                        RawRequestBody = rawRequestBody,
                        BodyValidationResult = RequestValidationResult.Unvalidated,
                        Exception = e,
                    };
                }

                // Validate body.
                try
                {
                    bodyValidationResult = ValidateBody(requestBody);
                }
                catch (Exception e)
                {
                    logger.LogDebug(e, ToolResources.ContextCreateBodyValidationErrorMessage);
                    return new FunctionRequestContext<TBody>(baseResult)
                    {
                        Status = RequestContextStatus.ValidationError,
                        RawRequestBody = rawRequestBody,
                        RequestBody = requestBody,
                        BodyValidationResult = RequestValidationResult.Unvalidated,
                        Exception = e,
                    };
                }
            }
            else
            {
                bodyValidationResult = RequestValidationResult.Ok;
            }

            RequestContextStatus status = bodyValidationResult?.Status == RequestValidationStatus.Passed
                ? RequestContextStatus.Success
                : RequestContextStatus.ValidationFailure;

            logger.LogDebug(string.Format(ToolResources.ContextCreateCompleteMessage, status));

            return new FunctionRequestContext<TBody>(baseResult)
            {
                Status = status,
                RawRequestBody = rawRequestBody,
                RequestBody = requestBody,
                BodyValidationResult = bodyValidationResult,
            };
        }

        /// <summary>
        /// Validate the body of a request.
        /// </summary>
        /// <param name="requestBody">The body to validate.</param>
        /// <returns>The validation result.</returns>
        public abstract RequestValidationResult ValidateBody(TBody requestBody);
    }
}
