using ExampleFunctionAppProject;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Unify.AzureFunctionAppTools.Preprocessing;

[assembly: FunctionsStartup(typeof(Startup))]

namespace ExampleFunctionAppProject
{
    /// <summary>
    /// All incoming requests are passed through Preprocessor methods. This Preprocessor method is used to check if the user is authenticated.
    /// </summary>
    public class AuthPreprocessor : IPreprocessor
    {
        public async Task<PreprocessorResult> Process(HttpRequest context, ILogger logger)
        {
            if (context.HttpContext.User?.Identity?.IsAuthenticated == true)
            {
                return PreprocessorResult.Continue;
            }

            return PreprocessorResult.Halt(new UnauthorizedResult());
        }
    }
}