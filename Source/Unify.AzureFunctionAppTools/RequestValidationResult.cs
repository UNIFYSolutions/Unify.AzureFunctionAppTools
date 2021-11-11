namespace Unify.AzureFunctionAppTools
{
    /// <summary>
    /// Results of validating some part of the request.
    /// </summary>
    public class RequestValidationResult
    {
        /// <summary>
        /// The validation status.
        /// </summary>
        public RequestValidationStatus Status { get; init; }

        /// <summary>
        /// The issues that were found during validation.
        /// </summary>
        public string[] Issues { get; init; } = new string[0];

        /// <summary>
        /// Create a successful validation result.
        /// </summary>
        public static RequestValidationResult Ok => new RequestValidationResult { Status = RequestValidationStatus.Passed };

        /// <summary>
        /// Create an unvalidation result.
        /// </summary>
        public static RequestValidationResult Unvalidated => new RequestValidationResult { Status = RequestValidationStatus.Unvalidated };
    }
}
