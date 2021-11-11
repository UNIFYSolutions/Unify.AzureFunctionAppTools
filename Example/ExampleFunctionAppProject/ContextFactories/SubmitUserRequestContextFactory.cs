using ExampleFunctionAppProject.Configurations;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using Unify.AzureFunctionAppTools;
using Unify.AzureFunctionAppTools.ExceptionHandling;
using Unify.AzureFunctionAppTools.Preprocessing;

namespace ExampleFunctionAppProject
{
    /// <summary>
    /// Reads the HTTP request creates a request context for use in the function handlers. Also performs validation on the headers, 
    /// query parameters and body as defined in the methods overriden from the base class.
    /// This reader uses the typed variant of <see cref="RequestContextFactoryBase"/> which is for reading requests with a body.
    /// </summary>
    public class SubmitUserRequestContextFactory : RequestContextFactoryBase<SubmitUserRequestBody>
    {
        private readonly IOptions<SubmitUserConfiguration> _SubmitUserConfig;

        /// <summary>
        /// Constructor for this factory.
        /// </summary>
        /// <param name="unhandledErrorFactory">Handler of uncaught errors.</param>
        /// <param name="preprocessorCollection">Collection of request preprocessors.</param>
        /// <param name="submitUserConfig">Configuration for submit user requests.</param>
        public SubmitUserRequestContextFactory(
            IUnhandledErrorFactory unhandledErrorFactory,
            IRequestPreprocessorCollection<SubmitUserRequestContextFactory> preprocessorCollection,
            IOptions<SubmitUserConfiguration> submitUserConfig) 
                : base(unhandledErrorFactory, preprocessorCollection)
        {
            _SubmitUserConfig = submitUserConfig ?? throw new ArgumentNullException(nameof(submitUserConfig));
        }

        /// <inheritdoc />
        public override RequestValidationResult ValidateHeaders(IDictionary<string, string[]> headers)
        {
            // Perform any header validation required.

            if (!headers.TryGetValue("correlationId", out string[] correlationIdParamValues) ||
                !correlationIdParamValues.Any())
            {
                return new RequestValidationResult
                {
                    Status = RequestValidationStatus.Failed,
                    Issues = new[]
                    {
                        "Correlation id header was not provided."
                    }
                };
            }

            return RequestValidationResult.Ok;
        }

        /// <inheritdoc />
        public override RequestValidationResult ValidateQueryParameters(IDictionary<string, string[]> queryParameters)
        {
            // This function requires no validation on query parameters.
            return RequestValidationResult.Ok;
        }

        /// <inheritdoc />
        public override RequestValidationResult ValidateBody(SubmitUserRequestBody requestBody)
        {
            // Perform any request body validation required.

            var issues = new List<string>();

            if (string.IsNullOrWhiteSpace(requestBody.Name))
            {
                issues.Add("Name is empty");
            }
            else if (requestBody.Name.Length > 100)
            {
                issues.Add("Name is too long.");
            }

            if (requestBody.Age < 0 || requestBody.Age > 200)
            {
                issues.Add("Age is not a valid age.");
            }

            if (requestBody.Start == null || requestBody.Start < _SubmitUserConfig.Value.MinStart)
            {
                issues.Add("Start date is missing or invalid.");
            }

            return new RequestValidationResult
            {
                Status = issues.Any() ? RequestValidationStatus.Failed : RequestValidationStatus.Passed,
                Issues = issues.ToArray()
            };
        }
    }
}
