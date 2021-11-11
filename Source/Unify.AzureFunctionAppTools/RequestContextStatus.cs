namespace Unify.AzureFunctionAppTools
{
    /// <summary>
    /// Status of a request context.
    /// </summary>
    public enum RequestContextStatus
    {
        /// <summary>
        /// The context was created successfully.
        /// </summary>
        Success,
        
        /// <summary>
        /// The body of the request could not be read from the request.
        /// </summary>
        BodyReadError,
        
        /// <summary>
        /// The body of the request could not be deserialized.
        /// </summary>
        BodyDeserializationError,

        /// <summary>
        /// An error occurred during request validation.
        /// </summary>
        ValidationError,

        /// <summary>
        /// Validation did not pass.
        /// </summary>
        ValidationFailure,

        /// <summary>
        /// A preprocessor has directed the request to be prematurely stopped.
        /// </summary>
        PreprocessorHalt,
    }
}
