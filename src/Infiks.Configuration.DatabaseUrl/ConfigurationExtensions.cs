using Microsoft.Extensions.Configuration;

namespace Infiks.Configuration.DatabaseUrl
{
    /// <summary>
    /// Extensions for <see cref="IConfigurationBuilder"/>.
    /// </summary>
    public static class ConfigurationExtensions
    {
        /// <summary>
        /// Reads a connection string from a database URL contained in a environment variable.
        /// </summary>
        /// <param name="builder">The configuration builder.</param>
        /// <param name="environmentVariable">The environment variable name, defaults to "DATABASE_URL".</param>
        /// <param name="connectionString">The connection string name, defaults to "DatabaseUrl".</param>
        /// <returns>The configuration builder.</returns>
        public static IConfigurationBuilder AddDatabaseUrl(this IConfigurationBuilder builder, string environmentVariable = null, string connectionString = null)
        {
            return builder
                .Add(new DatabaseUrlConfigurationSource(environmentVariable ?? "DATABASE_URL", connectionString ?? "DatabaseUrl"));
        }
    }
}
