using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Unify.AzureFunctionAppTools;
using Microsoft.Extensions.Configuration;

namespace ExampleFunctionAppProject.Functions
{
    public class GetUserFunction
    {
        private readonly GetUserRequestContextFactory _ContextFactory;
        private readonly GetUserFunctionHandler _Handler;

        public GetUserFunction(
            GetUserRequestContextFactory contextFactory,
            GetUserFunctionHandler handler,
            IConfiguration config)
        {
            _ContextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
            _Handler = handler ?? throw new ArgumentNullException(nameof(handler));
        }

        /// <summary>
        /// Example function that provides a HTTP GET api endpoint for submitting a users data. Route is `/api/v1/user` as specified in the custom route.
        /// </summary>
        /// <param name="req">
        /// Input request. Must have `HttpTrigger` attribute. Multiple HTTP methods (ie get, post, delete) can be provided if needed.
        /// `Route` parameter can be provided to supply a custom route instead of using the function name.
        /// </param>
        /// <param name="log">The logger.</param>
        /// <returns>
        /// Return an action result. 
        /// The result of the function. Maps to a HTTP result code.
        /// See https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.iactionresult?view=aspnetcore-3.1 for the derived types of `IActionResult`.
        /// </returns>
        [FunctionName("GetUserFunction")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/user")] HttpRequest req)
        {
            FunctionRequestContext context = await _ContextFactory.Create(req);
            return await _Handler.Handle(context);
        }
    }
}
