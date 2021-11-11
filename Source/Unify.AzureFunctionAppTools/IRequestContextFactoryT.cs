using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Unify.AzureFunctionAppTools
{
    /// <summary>
    ///  Create a request context from the content of a <see cref="HttpRequest"/> for use by a function.
    /// </summary>
    /// <typeparam name="TBody">Type of body for the request contains.</typeparam>
    public interface IRequestContextFactory<TBody>
    {
        /// <summary>
        /// Create the context.
        /// </summary>
        /// <param name="request">The request for which to create a context.</param>
        /// <param name="logger">The functions logger.</param>
        /// <returns>Context for the request.</returns>
        Task<FunctionRequestContext<TBody>> Create(HttpRequest request, ILogger logger);
    }
}
