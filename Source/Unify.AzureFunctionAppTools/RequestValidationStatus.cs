namespace Unify.AzureFunctionAppTools
{
    /// <summary>
    /// The state of validation.
    /// </summary>
    public enum RequestValidationStatus
    {
        /// <summary>
        /// The validation passed.
        /// </summary>
        Passed,

        /// <summary>
        /// The validation failed.
        /// </summary>
        Failed,

        /// <summary>
        /// Validation was not or could not be performed.
        /// </summary>
        Unvalidated
    }
}
