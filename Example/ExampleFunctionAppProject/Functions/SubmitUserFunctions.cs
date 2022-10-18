using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Unify.AzureFunctionAppTools;

namespace ExampleFunctionAppProject
{
    public class SubmitUserFunctions
    {
        private readonly SubmitUserRequestContextFactory _RequestReader;
        private readonly SubmitUserFunctionHandler _Handler;
        private readonly ILogger<SubmitUserFunctions> _Logger;

        public SubmitUserFunctions(
            SubmitUserRequestContextFactory requestReader,
            SubmitUserFunctionHandler handler,
            ILogger<SubmitUserFunctions> logger)
        {
            _RequestReader = requestReader ?? throw new ArgumentNullException(nameof(requestReader));
            _Handler = handler ?? throw new ArgumentNullException(nameof(handler));
            _Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Example function that provides a HTTP POST api endpoint for submitting a users data. Route is `/api/SubmitUserFunction` as custom route not provided.
        /// </summary>
        /// <param name="req">
        /// Input request. Must have `HttpTrigger` attribute. Multiple HTTP methods (ie get, post, delete) can be provided if needed.
        /// `Route` parameter can be provided to supply a custom route instead of useing the function name.
        /// </param>
        /// <param name="log">The logger.</param>
        /// <returns>
        /// Return an action result. 
        /// The result of the function. Maps to a HTTP result code.
        /// See https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.iactionresult?view=aspnetcore-3.1 for the derived types of `IActionResult`.
        /// </returns>
        [FunctionName("SubmitUserFunction")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req)
        {
            FunctionRequestContext<SubmitUserRequestBody> context = await _RequestReader.Create(req, _Logger);
            return await _Handler.Handle(context);
        }
    }
}
