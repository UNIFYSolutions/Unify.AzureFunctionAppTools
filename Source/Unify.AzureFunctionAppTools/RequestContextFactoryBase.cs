using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unify.AzureFunctionAppTools.ExceptionHandling;
using Unify.AzureFunctionAppTools.Preprocessing;
using Microsoft.Extensions.DependencyInjection;

namespace Unify.AzureFunctionAppTools
{
    /// <summary>
    /// Base request context factory, for requests without a body.
    /// </summary>
    public abstract class RequestContextFactoryBase : IRequestContextFactory
    {
        private readonly IEnumerable<IPreprocessor> _Preprocessors;
        private readonly IPreprocessorSelectiveExecutionRules _PreprocessorSelectiveExecutionRules;

        /// <summary>
        /// Constructor for this factory.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        protected RequestContextFactoryBase(IServiceProvider serviceProvider)
        {
            UnhandledErrorFactory = serviceProvider.GetRequiredService<IUnhandledErrorFactory>();
            _Preprocessors = serviceProvider.GetServices<IPreprocessor>();
            _PreprocessorSelectiveExecutionRules = serviceProvider.GetService<IPreprocessorSelectiveExecutionRules>();
        }

        /// <summary>
        /// Factory for creating unhandled error responses.
        /// </summary>
        protected IUnhandledErrorFactory UnhandledErrorFactory { get; }

        /// <inheritdoc />
        public async Task<FunctionRequestContext> Create(HttpRequest request, ILogger logger)
        {
            FunctionRequestContext context = await InnerCreate(request, logger);
            logger.LogDebug(string.Format(ToolResources.ContextCreateCompleteMessage, context.Status));
            return context;
        }

        /// <summary>
        /// Validate the headers of a request.
        /// </summary>
        /// <param name="headers">The headers to validate.</param>
        /// <returns>The validation result.</returns>
        public abstract RequestValidationResult ValidateHeaders(IDictionary<string, string[]> headers);

        /// <summary>
        /// Validate the query parameters of a request.
        /// </summary>
        /// <param name="queryParameters">The query parameters to validate.</param>
        /// <returns>The validation result.</returns>
        public abstract RequestValidationResult ValidateQueryParameters(IDictionary<string, string[]> queryParameters);

        /// <summary>
        /// Create the basic request context.
        /// </summary>
        protected async Task<FunctionRequestContext> InnerCreate(HttpRequest request, ILogger logger)
        {
            RequestValidationResult headerValidationResult;
            RequestValidationResult queryParamValidationResult;

            PreprocessorResult preprocessorResult = await RunPreprocessors(request, logger);

            if (!preprocessorResult.ShouldContinue)
            {
                logger.LogDebug(ToolResources.ContextCreatePreprocessorHaltMessage);
                return new FunctionRequestContext
                {
                    Status = RequestContextStatus.PreprocessorHalt,
                    Logger = logger,
                    PreprocessorHaltResponse = preprocessorResult.HaltResponse,
                    HeaderValidationResult = RequestValidationResult.Unvalidated,
                    QueryParameterValidationResult = RequestValidationResult.Unvalidated
                };
            }

            // Read headers.
            IDictionary<string, string[]> headers = ConvertStringDictionary(request.Headers);

            try
            {
                // Validate headers.
                headerValidationResult = ValidateHeaders(headers);
            }
            catch (Exception e)
            {
                logger.LogDebug(e, ToolResources.ContextCreateHeaderValidationErrorMessage);
                return new FunctionRequestContext
                {
                    Status = RequestContextStatus.ValidationError,
                    Request = request,
                    Logger = logger,
                    Headers = headers,
                    Exception = e,
                    HeaderValidationResult = RequestValidationResult.Unvalidated,
                    QueryParameterValidationResult = RequestValidationResult.Unvalidated
                };
            }

            // Read query parameters.
            IDictionary<string, string[]> queryParams = ConvertStringDictionary(request.Query);

            try
            {
                // Validate query parameters.
                queryParamValidationResult = ValidateQueryParameters(queryParams);
            }
            catch (Exception e)
            {
                logger.LogDebug(e, ToolResources.ContextCreateQueryParamValidationErrorMessage);
                return new FunctionRequestContext
                {
                    Status = RequestContextStatus.ValidationError,
                    Request = request,
                    Logger = logger,
                    Headers = headers,
                    QueryParameter = queryParams,
                    HeaderValidationResult = headerValidationResult,
                    QueryParameterValidationResult = RequestValidationResult.Unvalidated,
                    Exception = e,
                };
            }

            return new FunctionRequestContext
            {
                Status = headerValidationResult.Status == RequestValidationStatus.Passed && queryParamValidationResult.Status == RequestValidationStatus.Passed
                    ? RequestContextStatus.Success
                    : RequestContextStatus.ValidationFailure,
                Request = request,
                Logger = logger,
                Headers = headers,
                QueryParameter = queryParams,
                HeaderValidationResult = headerValidationResult,
                QueryParameterValidationResult = queryParamValidationResult,
                RequestMetadata = preprocessorResult.RequestMetadata,
            };
        }

        private async Task<PreprocessorResult> RunPreprocessors(HttpRequest target, ILogger logger)
        {
            PreprocessorResult aggregateResult = PreprocessorResult.Continue;

            foreach (IPreprocessor preprocessor in _Preprocessors)
            {
                if (!_PreprocessorSelectiveExecutionRules.CanExecute(preprocessor.GetType(), GetType()))
                    continue;

                try
                {
                    PreprocessorResult thisResult = await preprocessor.Process(target, logger);

                    if (thisResult == null)
                    {
                        string message = string.Format(ToolResources.NullPreprocessorResultMessage, preprocessor.GetType().FullName);
                        logger.LogDebug(message);
                        return PreprocessorResult.Halt(UnhandledErrorFactory.Create(message));
                    }

                    if (!thisResult.ShouldContinue)
                        return thisResult;

                    // Append metadata
                    foreach (KeyValuePair<string, object> pair in thisResult.RequestMetadata)
                    {
                        aggregateResult.RequestMetadata[pair.Key] = pair.Value;
                    }
                }
                catch (Exception e)
                {
                    string message = string.Format(ToolResources.PreprocessorErrorMessage, preprocessor.GetType().Name);
                    logger.LogDebug(e, message);
                    return PreprocessorResult.Halt(UnhandledErrorFactory.Create(message, e));
                }
            }

            return aggregateResult;
        }

        private IDictionary<string, string[]> ConvertStringDictionary(IEnumerable<KeyValuePair<string, StringValues>> original)
        {
            IDictionary<string, string[]> converted = original?.ToDictionary(
                headerPair => headerPair.Key,
                headerPair => headerPair.Value.ToArray());

            return converted ?? new Dictionary<string, string[]>();
        }
    }
}
