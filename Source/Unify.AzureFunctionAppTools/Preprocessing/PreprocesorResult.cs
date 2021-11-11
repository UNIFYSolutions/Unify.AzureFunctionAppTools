using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Unify.AzureFunctionAppTools.Preprocessing
{
    /// <summary>
    /// Result of a preprocessor execution.
    /// </summary>
    public class PreprocessorResult
    {
        /// <summary>
        /// If the request should continue processing.
        /// </summary>
        public bool ShouldContinue { get; init; }

        /// <summary>
        /// A result to return if <see cref="Continue"/> is false. 
        /// </summary>
        public IActionResult HaltResponse { get; init; }

        /// <summary>
        /// Additional data related to the request.
        /// </summary>
        public Dictionary<string, object> RequestMetadata { get; } = new();

        /// <summary>
        /// A continue result.
        /// </summary>
        /// <returns>THe result.</returns>
        public static PreprocessorResult Continue => new() { ShouldContinue = true };

        /// <summary>
        /// A halt result.
        /// </summary>
        /// <returns>THe result.</returns>
        public static PreprocessorResult Halt(IActionResult response) => new() { ShouldContinue = false, HaltResponse = response };
    }
}