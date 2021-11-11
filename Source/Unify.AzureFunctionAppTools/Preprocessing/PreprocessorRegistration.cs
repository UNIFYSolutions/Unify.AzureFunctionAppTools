using System;
using System.Collections.Generic;

namespace Unify.AzureFunctionAppTools.Preprocessing
{
    /// <summary>
    /// A registration of a preprocessor to preprocessor configuration.
    /// </summary>
    public class PreprocessorRegistration
    {
        internal readonly Type PreprocessorType;
        internal readonly Func<IServiceProvider, IPreprocessor> FactoryFunction;
        internal readonly List<Type> ForTypes = new List<Type>();
        internal readonly List<Type> NotForTypes = new List<Type>();

        /// <summary>
        /// Constructor for this registration.
        /// </summary>
        /// <param name="preprocessorType">The preprocessor type.</param>
        public PreprocessorRegistration(Type preprocessorType)
        {
            PreprocessorType = preprocessorType ?? throw new ArgumentNullException(nameof(preprocessorType));
        }

        /// <summary>
        /// Constructor for this registration.
        /// </summary>
        /// <param name="factoryFunction">Factory function for the preprocessor.</param>
        public PreprocessorRegistration(Func<IServiceProvider, IPreprocessor> factoryFunction)
        {
            FactoryFunction = factoryFunction ?? throw new ArgumentNullException(nameof(factoryFunction));
        }

        /// <summary>
        /// Register a context factory as one that the registered preprocessor should execute for. If any context factories are registered
        /// for a preprocessor, none others will execute the preprocessor unless they too are registerd for the preprocessor using this method.
        /// </summary>
        /// <typeparam name="TContextFactory">Type of context factory.</typeparam>
        /// <returns>The preprocessor registration.</returns>
        public PreprocessorRegistration For<TContextFactory>()
            where TContextFactory : IRequestContextFactory
        {
            ForTypes.Add(typeof(TContextFactory));
            return this;
        }

        /// <summary>
        /// Register a context factory as one that the registered preprocessor should not execute for. Takes precidence over thoes context
        /// factories registerd with <see cref="For{TContextFactory}"/> for the preprocessor.
        /// </summary>
        /// <typeparam name="TContextFactory">Type of context factory.</typeparam>
        /// <returns>The preprocessor registration.</returns>
        public PreprocessorRegistration NotFor<TContextFactory>()
            where TContextFactory : IRequestContextFactory
        {
            NotForTypes.Add(typeof(TContextFactory));
            return this;
        }
    }
}
