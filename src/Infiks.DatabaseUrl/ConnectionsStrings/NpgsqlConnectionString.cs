using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace Infiks.DatabaseUrl.ConnectionsStrings
{
    /// <summary>
    /// Converts a <see cref="DatabaseUrl"/> into a connection string.
    /// </summary>
    public class NpgsqlConnectionString
    {
        /// <summary>
        /// The connection string builder.
        /// </summary>
        private readonly DbConnectionStringBuilder _builder;

        /// <summary>
        /// Initializes a new <see cref="NpgsqlConnectionString"/>.
        /// </summary>
        /// <param name="databaseUrl">The database URL.</param>
        public NpgsqlConnectionString(DatabaseUrl databaseUrl)
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

            foreach (var item in GetOptions(databaseUrl.Options))
            {
                _builder.Add(item.Key, item.Value);
            }
        }

        /// <summary>
        /// Transforms the given options into Npgsql options.
        /// </summary>
        /// <param name="options">The key-value options.</param>
        /// <returns>The transformed options.</returns>
        private IEnumerable<KeyValuePair<string, string>> GetOptions(IDictionary<string, string> options)
        {
            foreach (var item in options)
            {
                yield return GetOption(item);
            }
        }

        /// <summary>
        /// Transforms the given option into an Npgsql option.
        /// </summary>
        /// <param name="option">The key-value option.</param>
        /// <returns>The transformed option.</returns>
        private KeyValuePair<string, string> GetOption(KeyValuePair<string, string> option)
        {
            switch (option.Key)
            {
                case "keepalives":
                    var boolValue = int.TryParse(option.Value, out var b) && b > 0 ? "True" : "False";
                    return new KeyValuePair<string, string>(MapOption(option.Key), boolValue);
                case "keepalives_idle":
                case "keepalives_interval":
                    var milliseconds = int.TryParse(option.Value, out var s) ? $"{s * 1000}" : option.Value;
                    return new KeyValuePair<string, string>(MapOption(option.Key), milliseconds);
                default:
                    return new KeyValuePair<string, string>(MapOption(option.Key), option.Value);
            }
        }

        /// <summary>
        /// Maps the options name to an Npgsql option name.
        /// </summary>
        /// <param name="key">The options name.</param>
        /// <returns>The Npgsql option name.</returns>
        private string MapOption(string key)
        {
            switch (key)
            {
                case "passfile":
                    return "Passfile";
                case "connect_timeout":
                    return "Timeout";
                case "client_encoding":
                    return "Client Encoding";
                case "options":
                    return "Options";
                case "application_name":
                    return "Application Name";
                case "keepalives":
                    return "Tcp Keepalive";
                case "keepalives_idle":
                    return "Tcp Keepalive Time";
                case "keepalives_interval":
                    return "Tcp Keepalive Interval";
                case "sslmode":
                    return "SSL Mode";
                case "sslcert":
                    return "SSL Certificate";
                case "sslkey":
                    return "SSL Key";
                case "sslpassword":
                    return "SSL Password";
                case "sslrootcert":
                    return "Root Certificate";
                case "target_session_attrs":
                    return "Target Session Attributes";
                default:
                    return key;
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
