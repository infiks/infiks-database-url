using System;
using System.Data.Common;
using System.Linq;

namespace Infiks.DatabaseUrl.ConnectionsStrings
{
    /// <summary>
    /// Converts a <see cref="DatabaseUrl"/> into a connection string.
    /// </summary>
    public class GenericConnectionString
    {
        /// <summary>
        /// The connection string builder.
        /// </summary>
        private readonly DbConnectionStringBuilder _builder;

        /// <summary>
        /// Initializes a new <see cref="GenericConnectionString"/>.
        /// </summary>
        /// <param name="databaseUrl">The database URL.</param>
        public GenericConnectionString(DatabaseUrl databaseUrl)
        {
            if (databaseUrl == null)
            {
                throw new ArgumentNullException(nameof(databaseUrl));
            }

            var hosts = databaseUrl.Hosts
                .Select(x => x.Port.HasValue ? $"{x.Host}:{x.Port}" : x.Host)
                .Aggregate("", (s, v) => s.Length > 0 ? $"{s},{v}" : v);

            _builder = new DbConnectionStringBuilder();

            if (!string.IsNullOrEmpty(hosts))
            {
                _builder.Add("Host", hosts);
            }

            if (!string.IsNullOrEmpty(databaseUrl.Username))
            {
                _builder.Add("Username", databaseUrl.Username);
            }

            if (!string.IsNullOrEmpty(databaseUrl.Password))
            {
                _builder.Add("Password", databaseUrl.Password);
            }

            if (!string.IsNullOrEmpty(databaseUrl.Database))
            {
                _builder.Add("Database", databaseUrl.Database);
            }

            foreach (var item in databaseUrl.Options)
            {
                _builder.Add(item.Key, item.Value);
            }
        }

        /// <summary>
        /// Returns the database connection string.
        /// </summary>
        /// <returns>The connection string.</returns>
        public override string ToString()
        {
            return _builder.ConnectionString;
        }
    }
}
