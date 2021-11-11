namespace ExampleFunctionAppProject
{
    /// <summary>
    /// Response body returned by HTTP functions when validation fails.
    /// </summary>
    public class ValidationFailureResponseBody : MessageResponseBody
    {
        /// <summary>
        /// The JSON string body of the request.
        /// </summary>
        public string RequestBody { get; set; }

        /// <summary>
        /// A list of issues that were found during validation.
        /// </summary>
        public string[] Issues { get; set; }
    }
}
