# Azure Function Apps Development Guide

## TODO

* clean up
    * comments - missing params, typeparams, returns or altogether
    * string literal usage to constant/resource file
* create a few example preprocessors
* create any preprocessors for common cases
* unit test tools lib
* azure config loading
* the rest of this documentation

## Quickstart

### Project Setup

1. Create `Azure Function` project, type `Azure Function v3 (.NET Core)`
1. Update package reference `Microsoft.NET.Sdk.Functions` to latest (currently v3.0.11).
1. Add package reference to `Microsoft.Azure.Functions.Extensions`
1. Add package reference to `Unify.AzureFunctionAppTools`

### Startup

1. Create a class `Startup`, deriving from `Microsoft.Azure.Functions.Extensions.DependencyInjection.FunctionsStartup`.
1. Override the `Configure(IFunctionsHostBuilder)` method.
1. Register custom uncaught error factory by adding:

    ```c#
    builder.Services.AddUnhandledErrorFactory<MyCustomFactory>();
    ```

    or use the default one by adding:

    ```c#
    builder.Services.AddUnhandledErrorFactory();
    ```

1. Register any additional DI services or configuration.

### Functions

1. Create new HTTP function, authorization level `Anonymous`
1. Rename function class, `FunctionName` attribute
1. Setup `HttpTrigger` attribute as requiredparameter
1. Remote `static` keyword from class and `Run` method
1. Remote all parameters from `Run` method except `HttpRequest req`.
1. Add a constructor to the function class
1. Create a class to be the model of the request body, if needed.
1. Create a context factory, derived from `Unify.AzureFunctionAppToolsRequestContextFactoryBase` or `Unify.AzureFunctionAppToolsRequestContextFactoryBase<TBody>`with any required validation.
1. Create a handler, derived from `Unify.AzureFunctionAppTools.RequestHandlerBase` or `Unify.AzureFunctionAppTools.RequestHandlerBase<TBody>` with the request/error/failure handling logic
1. Add the created context factory and handler as constructor parameters to the function. Replace the body of the functions `Run` method with:

    ```c#
    FunctionRequestContext context = await _ContextFactory.Create(req);
    return await _Handler.Handle(context);
    ```

1. Register the context factory and handler with the `AddFunctionAppTools<TContextFactory, THandler>` or `AddFunctionAppTools<TContextFactory, THandler, TBody>` from `Unify.AzureFunctionAppTools.FunctionAppStartupExtensions` in the `Startup.Configure(IFunctionHostBuilder)` method. Add any request or handler preprocessor using the optional parameters.
