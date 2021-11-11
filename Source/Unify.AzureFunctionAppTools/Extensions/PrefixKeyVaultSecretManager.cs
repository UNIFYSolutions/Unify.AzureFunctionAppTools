using System;
using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Configuration;

namespace Unify.AzureFunctionAppTools.Extensions
{
    /// <summary>
    /// Provides a secret manager for KeyVault which uses prefixes for auto loading secrets.
    /// </summary>
    public class PrefixKeyVaultSecretManager : KeyVaultSecretManager
    {
        private const string DefaultStoredKeyDelimiter = "--";

        private readonly string _Prefix;
        private readonly string _StoredKeyDelimiter;

        /// <summary>
        /// Creates an instance of the <see cref="PrefixKeyVaultSecretManager"/>.
        /// </summary>
        /// <param name="prefix">Prefix of the stored keys.</param>
        /// <param name="keyDelimiter">Optional override of the key section delimiter.</param>
        public PrefixKeyVaultSecretManager(string prefix, string keyDelimiter = DefaultStoredKeyDelimiter)
        {
            if (string.IsNullOrEmpty(prefix)) throw new ArgumentNullException(nameof(prefix));
            if (string.IsNullOrEmpty(keyDelimiter)) throw new ArgumentNullException(nameof(keyDelimiter));

            _Prefix = $"{prefix}-";
            _StoredKeyDelimiter = keyDelimiter;
        }

        /// <inheritdoc />
        public override bool Load(SecretProperties secret) => secret.Name.StartsWith(_Prefix);

        /// <inheritdoc />
        public override string GetKey(KeyVaultSecret secret) =>
            secret.Properties.Name
                .Substring(_Prefix.Length)
                .Replace(_StoredKeyDelimiter, ConfigurationPath.KeyDelimiter);
    }
}