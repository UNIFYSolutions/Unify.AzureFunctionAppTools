using ExampleFunctionAppProject;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Unify.AzureFunctionAppTools.Preprocessing;

[assembly: FunctionsStartup(typeof(Startup))]

namespace ExampleFunctionAppProject
{
    public class AuthPreprocessor : IRequestPreprocessor
    {
        public async Task<PreprocesorResult> Process(HttpRequest context)
        {
            if (context.HttpContext.User?.Identity?.IsAuthenticated != true)
            {
                return PreprocesorResult.Continue;
            }

            return PreprocesorResult.Halt(new UnauthorizedResult());
        }
    }
}
