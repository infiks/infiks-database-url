using Microsoft.Extensions.Configuration;
using System;

namespace Infiks.Configuration.DatabaseUrl
{
    /// <summary>
    /// Source configuration for database URLs.
    /// </summary>
    public class DatabaseUrlConfigurationSource : IConfigurationSource
    {
        /// <summary>
        /// The name of the connection string.
        /// </summary>
        private readonly string _name;

        /// <summary>
        /// The name of the environment variable.
        /// </summary>
        private readonly string _environmentVariable;

        /// <summary>
        /// Initializes a new <see cref="DatabaseUrlConfigurationSource"/>.
        /// </summary>
        /// <param name="environmentVariable">The name of the environment variable.</param>
        /// <param name="name">The name of the connection string.</param>
        public DatabaseUrlConfigurationSource(string environmentVariable, string name)
        {
            _name = name ?? throw new ArgumentNullException(nameof(name));
            _environmentVariable = environmentVariable ?? throw new ArgumentNullException(nameof(environmentVariable));
        }

        /// <inheritdoc/>
        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new DatabaseUrlConfigurationProvider(_environmentVariable, _name);
        }
    }
}
