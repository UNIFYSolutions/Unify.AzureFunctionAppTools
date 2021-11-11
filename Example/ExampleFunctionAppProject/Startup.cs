using ExampleFunctionAppProject.Configurations;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MockUserSource;
using Unify.AzureFunctionAppTools;

namespace ExampleFunctionAppProject
{
    /// <summary>
    /// Startup class. Must derive from <see cref="FunctionsStartup"/> and override <see cref="FunctionsStartup.Configure(IFunctionsHostBuilder)"/>.
    /// </summary>
    public class Startup : FunctionsStartup
    {
        /// <summary>
        /// Register function tools using `Add...` extension methods.
        /// Register any dependencies here for use in those resources, as well as any required ASP.NET Core middleware.
        /// </summary>
        public override void Configure(IFunctionsHostBuilder builder)
        {
            // Register the unhandled error factory, either the default or specifying a custom one. This is required.
            builder.Services.AddUnhandledErrorFactory();

            builder.Services.AddLogging();

            // Register the Unify function app tools for each function.
            builder.Services.AddFunctionAppTools<GetUserRequestContextFactory, GetUserFunctionHandler>();
            builder.Services.AddFunctionAppTools<SubmitUserRequestContextFactory, SubmitUserFunctionHandler, SubmitUserRequestBody>();

            // Configuration (local.settings.json when running locally) can be accessed directly in DI resources though `IConfiguration` or
            // setup here, either binding a section to a configuration class or manually, and accessed though DI with `IOptions<TConfig>`.
            builder.Services.AddOptions<SubmitUserConfiguration>().Configure<IConfiguration>((options, config) => config.Bind("submitUser"));

            // Register any other required resources.
            builder.Services.AddSingleton<IUserSource, UserSource>();
        }
    }
}
