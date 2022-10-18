using System;
using System.Collections.Generic;
using Unify.AzureFunctionAppTools;

namespace ExampleFunctionAppProject
{
    /// <summary>
    /// Reads the HTTP request creates a request context for use in the function handlers. Also performs validation on the headers, 
    /// query parameters and body as defined in the methods overriden from the base class.
    /// This factory uses the un-typed variant of <see cref="RequestContextFactoryBase"/> which is for reading requests without a body.
    /// </summary>
    public class GetUserRequestContextFactory : RequestContextFactoryBase
    {
        /// <summary>
        /// Constructor for this factory.
        /// </summary>
        /// <param name="unhandledErrorFactory">Handler of uncaught errors.</param>
        /// <param name="preprocessorCollection">Preprocessors to run.</param>
        public GetUserRequestContextFactory(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        /// <inheritdoc />
        public override RequestValidationResult ValidateHeaders(IDictionary<string, string[]> headers)
        {
            // This function requires no validation on headers.
            return RequestValidationResult.Ok;
        }

        /// <inheritdoc />
        public override RequestValidationResult ValidateQueryParameters(IDictionary<string, string[]> queryParameters)
        {
            // Perform any query parameter validation

            if (!queryParameters.TryGetValue(FunctionConstants.UserIdParamName, out string[] userIdValues) ||
                userIdValues.Length != 1 ||
                string.IsNullOrWhiteSpace(userIdValues[0]))
            {
                return new RequestValidationResult
                {
                    Status = RequestValidationStatus.Failed,
                    Issues = new[]
                    {
                        "No user id id provided."
                    }
                };
            }

            if (!Guid.TryParse(userIdValues[0], out _))
            {
                return new RequestValidationResult
                {
                    Status = RequestValidationStatus.Failed,
                    Issues = new[]
                    {
                        "User id was in an invalid format."
                    }
                };
            }

            return RequestValidationResult.Ok;
        }
    }
}
