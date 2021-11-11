using System;

namespace Unify.AzureFunctionAppTools.Preprocessing
{
    /// <summary>
    /// Rules for if a preprocessor should be run by a context factory.
    /// </summary>
    internal interface IPreprocessorSelectiveExecutionRules
    {
        /// <summary>
        /// If a preprocessor should run for a context factory.
        /// </summary>
        /// <param name="preprocessorType">The type of preprocessor.</param>
        /// <param name="contextFactoryType">The type of context factory.</param>
        /// <returns>If the preprocessor should be run by the context factory.</returns>
        bool CanExecute(Type preprocessorType, Type contextFactoryType);
    }
}