using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using Unify.AzureFunctionAppTools.ExceptionHandling;
using Unify.AzureFunctionAppTools.Preprocessing;

namespace Unify.AzureFunctionAppTools
{
    /// <summary>
    /// Extensions for use in function app startup classes.
    /// </summary>
    public static class FunctionAppStartupExtensions
    {
        /// <summary>
        /// Register function app tools for a function without a request body.
        /// </summary>
        /// <typeparam name="TContextFactory">Type of context factory.</typeparam>
        /// <typeparam name="THandler">Type of the function handler.</typeparam>
        /// <param name="services">The DI service collection to register to.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection AddFunctionAppTools<TContextFactory, THandler>(this IServiceCollection services)
            where TContextFactory : class, IRequestContextFactory
            where THandler : RequestHandlerBase
        {
            services.TryAddTransient<TContextFactory>();
            services.TryAddTransient<THandler>();

            return services;
        }

        /// <summary>
        /// Register function app tools for a function with a request body.
        /// </summary>
        /// <typeparam name="TContextFactory">Type of context factory.</typeparam>
        /// <typeparam name="THandler">Type of the function handler.</typeparam>
        /// <typeparam name="TBody">Type of the body the function request accepts.</typeparam>
        /// <param name="services">The DI service collection to register to.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection AddFunctionAppTools<TContextFactory, THandler, TBody>(this IServiceCollection services)
            where TContextFactory : class, IRequestContextFactory
            where THandler : RequestHandlerBase<TBody>
        {
            services.TryAddTransient<TContextFactory>();
            services.TryAddTransient<THandler>();

            return services;
        }

        /// <summary>
        /// Register the default unhandled error factory for dependency injection.
        /// </summary>
        /// <param name="services">The service collection to register with.</param>
        /// <param name="includeExceptionOverride">An override value for including exception details in the factories produced response. Null uses the default, which is only in an development environment.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection AddUnhandledErrorFactory(this IServiceCollection services, bool? includeExceptionOverride = null)
        {
            return services.AddTransient<IUnhandledErrorFactory>(sp =>
            {
                bool includeException = includeExceptionOverride == null 
                    ? sp.GetRequiredService<IHostingEnvironment>().IsDevelopment() 
                    : includeExceptionOverride.Value;

                return new DefaultUnhandledErrorFactory(sp.GetService<ILogger<IUnhandledErrorFactory>>(), includeException);
            });
        }

        /// <summary>
        /// Register the default unhandled error factory for dependency injection.
        /// </summary>
        /// <typeparam name="TFactory">The factory type.</typeparam>
        /// <param name="services">The service collection to register with.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection AddUnhandledErrorFactory<TFactory>(this IServiceCollection services)
            where TFactory : class, IUnhandledErrorFactory
        {
            return services.AddTransient<IUnhandledErrorFactory, TFactory>();
        }

        /// <summary>
        /// Register the default unhandled error factory for dependency injection.
        /// </summary>
        /// <typeparam name="TFactory">The factory type.</typeparam>
        /// <param name="services">The service collection to register with.</param>
        /// <param name="implementationFactory">Factory function for creating the factory.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection AddUnhandledErrorFactory<TFactory>(this IServiceCollection services, Func<IServiceProvider, TFactory> implementationFactory)
            where TFactory : class, IUnhandledErrorFactory
        {
            return services.AddTransient<IUnhandledErrorFactory, TFactory>(implementationFactory);
        }

        /// <summary>
        /// Setup the preprocesses for the service.
        /// </summary>
        /// <param name="services">The services collection.</param>
        /// <param name="configAction">The action where preprocessor setup configuration will occur.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection SetupPreprocessors(this IServiceCollection services, Action<PreprocessorConfiguration> configAction)
        {
            var config = new PreprocessorConfiguration();
            configAction(config);

            var allowedTypes = new Dictionary<Type, HashSet<Type>>();
            var denyedTypes = new Dictionary<Type, HashSet<Type>>();

            foreach (PreprocessorRegistration registration in config.PreprocessorRegistrations)
            {
                if (registration.FactoryFunction != null)
                    services.AddTransient(registration.FactoryFunction);
                else
                    services.AddTransient(typeof(IPreprocessor), registration.PreprocessorType);

                if (registration.ForTypes.Any())
                    allowedTypes[registration.PreprocessorType] = new HashSet<Type>(registration.ForTypes);
                
                if (registration.NotForTypes.Any())
                    denyedTypes[registration.PreprocessorType] = new HashSet<Type>(registration.NotForTypes);
            }

            services.AddSingleton<IPreprocessorSelectiveExecutionRules>(new PreprocessorSelectiveExecutionRules(allowedTypes, denyedTypes));

            return services;
        }
    }
}
