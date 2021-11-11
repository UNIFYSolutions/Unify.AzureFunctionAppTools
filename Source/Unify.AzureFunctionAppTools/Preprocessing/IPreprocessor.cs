using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Unify.AzureFunctionAppTools.Preprocessing
{
    /// <summary>
    /// Preprocessor for http requests.
    /// </summary>
    public interface IPreprocessor
    {
        /// <summary>
        /// Executes the preprocessor.
        /// </summary>
        /// <param name="target">Target to run processing on.</param>
        /// <param name="logger">The function logger.</param>
        /// <returns>Processing result.</returns>
        Task<PreprocessorResult> Process(HttpRequest target, ILogger logger);
    }
}
