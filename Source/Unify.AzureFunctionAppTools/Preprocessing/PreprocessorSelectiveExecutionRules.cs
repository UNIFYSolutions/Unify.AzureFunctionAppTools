using System;
using System.Collections.Generic;

namespace Unify.AzureFunctionAppTools.Preprocessing
{
    /// <summary>
    /// Rules for if a preprocessor should be run by a context factory.
    /// </summary>
    internal class PreprocessorSelectiveExecutionRules : IPreprocessorSelectiveExecutionRules
    {
        private readonly IDictionary<Type, HashSet<Type>> _AllowedTypes;
        private readonly IDictionary<Type, HashSet<Type>> _DenyedTypes;

        /// <summary>
        /// Constructor for these rules.
        /// </summary>
        /// <param name="allowedTypes">The context factory types that are allowed for preprocessors.</param>
        /// <param name="denyedTypes">The context factory types that are denyed for preprocessors.</param>
        public PreprocessorSelectiveExecutionRules(
            IDictionary<Type, HashSet<Type>> allowedTypes,
            IDictionary<Type, HashSet<Type>> denyedTypes)
        {
            _AllowedTypes = allowedTypes ?? throw new ArgumentNullException(nameof(allowedTypes));
            _DenyedTypes = denyedTypes ?? throw new ArgumentNullException(nameof(denyedTypes));

            // Normalize allowed type dictionary contents
            foreach (var key in _AllowedTypes.Keys)
            {
                if (_AllowedTypes[key] == null || _AllowedTypes[key].Count == 0)
                {
                    _AllowedTypes.Remove(key);
                }
            }

            // Normalize denyed type dictionary contents
            foreach (var key in _DenyedTypes.Keys)
            {
                if (_DenyedTypes[key] == null || _DenyedTypes[key].Count == 0)
                {
                    _DenyedTypes.Remove(key);
                }
            }
        }

        /// <inheritdoc />
        public bool CanExecute(Type preprocessorType, Type contextFactoryType)
        {
            // If there are any deny types for a preprocessor, the context factory type cannot be one of them.
            if (_DenyedTypes.TryGetValue(preprocessorType, out HashSet<Type> denyedTypes) && denyedTypes.Contains(contextFactoryType))
                return false;

            // If there are any allow types for a preprocessor, the context factory type must be one of them.
            if (_AllowedTypes.TryGetValue(preprocessorType, out HashSet<Type> allowedTypes) && !allowedTypes.Contains(contextFactoryType))
                return false;

            return true;

        }
    }
}
