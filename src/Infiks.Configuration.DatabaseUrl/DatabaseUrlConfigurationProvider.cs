using Infiks.DatabaseUrl;
using Microsoft.Extensions.Configuration;
using System;

namespace Infiks.Configuration.DatabaseUrl
{
    /// <summary>
    /// Configuration provider for database URLs.
    /// </summary>
    public class DatabaseUrlConfigurationProvider : ConfigurationProvider
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
        /// The parent key for connection strings.
        /// </summary>
        private readonly string _connectionStringsKey = "ConnectionStrings";

        /// <summary>
        /// Initializes a new <see cref="DatabaseUrlConfigurationProvider"/>.
        /// </summary>
        /// <param name="environmentVariable">The name of the environment variable.</param>
        /// <param name="name">The name of the connection string.</param>
        public DatabaseUrlConfigurationProvider(string environmentVariable, string name)
        {
            _name = name ?? throw new ArgumentNullException(nameof(name));
            _environmentVariable = environmentVariable ?? throw new ArgumentNullException(nameof(environmentVariable));
        }

        /// <summary>
        /// Loads the database URL from the process environment.
        /// </summary>
        public override void Load()
        {
            var databaseUrl = Environment.GetEnvironmentVariable(_environmentVariable);
            if (!string.IsNullOrEmpty(databaseUrl))
            {
                var key = ConfigurationPath.Combine(_connectionStringsKey, _name);
                var connectionString = DatabaseUrlConverter.Convert(databaseUrl);
                Data[key] = connectionString;
            }
        }
    }
}
