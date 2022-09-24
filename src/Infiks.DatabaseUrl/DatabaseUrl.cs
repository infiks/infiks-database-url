using Infiks.DatabaseUrl.ConnectionsStrings;
using System;
using System.Collections.Generic;

namespace Infiks.DatabaseUrl
{
    /// <summary>
    /// Database connection info.
    /// </summary>
    public class DatabaseUrl
    {
        /// <summary>
        /// The original database URL.
        /// </summary>
        public string OriginalString { get; set; }

        /// <summary>
        /// The database connection scheme.
        /// </summary>
        public string Scheme { get; set; }

        /// <summary>
        /// The database connection user name.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// The database connection password.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// The database name.
        /// </summary>
        public string Database { get; set; }

        /// <summary>
        /// The database hosts. When the first host is not available, the second host is tried, then the third, etc.
        /// </summary>
        public IList<DatabaseHost> Hosts { get; set; }

        /// <summary>
        /// The database connection options.
        /// </summary>
        public IDictionary<string, string> Options { get; set; }

        /// <summary>
        /// Converts the database URL to a connection string.
        /// </summary>
        /// <returns>The connection string.</returns>
        public string ToConnectionString()
        {
            switch (Scheme)
            {
                case "postgres":
                case "postgresql":
                    return new NpgsqlConnectionString(this).ToString();
                default:
                    return new GenericConnectionString(this).ToString();
            }
        }
    }
}
