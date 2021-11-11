namespace Unify.AzureFunctionAppTools
{
    /// <summary>
    /// Context of a function request.
    /// </summary>
    /// <typeparam name="TBody">The type of body the request contains.</typeparam>
    public class FunctionRequestContext<TBody> : FunctionRequestContext
    {
        /// <summary>
        /// Constructor for this context.
        /// </summary>
        public FunctionRequestContext()
        {
        }

        /// <summary>
        /// Constructor for creating typed context from a untyped one.
        /// </summary>
        /// <param name="context">The untyped context.</param>
        public FunctionRequestContext(FunctionRequestContext context)
        {
            Status = context.Status;
            Logger = context.Logger;
            Request = context.Request;
            Headers = context.Headers;
            QueryParameter = context.QueryParameter;
            HeaderValidationResult = context.HeaderValidationResult;
            QueryParameterValidationResult = context.QueryParameterValidationResult;
            Exception = context.Exception;
            RequestMetadata = context.RequestMetadata;
            PreprocessorHaltResponse = context.PreprocessorHaltResponse;
        }

        /// <summary>
        /// The raw text of the request.
        /// </summary>
        public string RawRequestBody { get; init; }

        /// <summary>
        /// The deserialized request body.
        /// </summary>
        public TBody RequestBody { get; init; }

        /// <summary>
        /// The result of the body validation.
        /// </summary>
        public RequestValidationResult BodyValidationResult { get; init; }
    }
}
