using System.Collections.Generic;

namespace Unify.AzureFunctionAppTools.Preprocessing
{
    /// <summary>
    /// The preprocessor configuration for the service. 
    /// </summary>
    public class PreprocessorConfiguration
    {
        internal readonly List<PreprocessorRegistration> PreprocessorRegistrations = new List<PreprocessorRegistration>();

        /// <summary>
        /// Add a preprocessor to the service.
        /// </summary>
        /// <typeparam name="TPreprocessor">The type of preprocessor to add.</typeparam>
        /// <returns>The preprocessor registration, which can be use to modify how the preprocessor is registered.</returns>
        public PreprocessorRegistration AddPreprocesssor<TPreprocessor>()
            where TPreprocessor : IPreprocessor
        {
            var reg = new PreprocessorRegistration(typeof(TPreprocessor));
            PreprocessorRegistrations.Add(reg);
            return reg;
        }
    }
}
