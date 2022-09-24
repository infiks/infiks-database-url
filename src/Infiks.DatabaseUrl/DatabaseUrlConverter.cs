using System;
using System.Linq;
using System.Collections.Generic;
using System.Web;
using System.Text.RegularExpressions;

namespace Infiks.DatabaseUrl
{
    /// <summary>
    /// Class for converting a database URL into a connection string.
    /// </summary>
    public static class DatabaseUrlConverter
    {
        /// <summary>
        /// Takes a database URL in the form of <c>scheme://[username[:password]@]host1[:port1][,host2[:port2],...[,hostN[:portN]]][/[endpoint]][?options]</c>.
        /// And converts it into a connection string in the form of <c>Host=server;Username:john;Password=secret</c>.
        /// </summary>
        /// <param name="databaseUrl">The database URL.</param>
        /// <returns>The connection string.</returns>
        public static string Convert(string databaseUrl)
        {
            var dbUrl = Parse(databaseUrl);
            return dbUrl.ToConnectionString();
        }

        /// <summary>
        /// Takes a database URL in the form of <c>scheme://[username[:password]@]host1[:port1][,host2[:port2],...[,hostN[:portN]]][/[endpoint]][?options]</c>.
        /// </summary>
        /// <param name="databaseUrl">The database URL.</param>
        /// <returns>The parsed <see cref="DatabaseUrl"/>.</returns>
        /// <exception cref="ArgumentException"></exception>
        public static DatabaseUrl Parse(string databaseUrl)
        {
            // Implementation based on: https://github.com/sindilevich/connection-string-parser/blob/master/src/connection-string-parser.ts

            var pattern =
                "^\\s*" + // Optional whitespace padding at the beginning of the line
                "(?<Scheme>[^:]+)://" + // Scheme (Group 1)
                "(?:(?<Username>[^:@,/?=&]+)(?::(?<Password>[^:@,/?=&]+))?@)?" + // User (Group 2) and Password (Group 3)
                "(?<Hosts>[^@/?=&]+)?" + // Host address(es) (Group 4)
                "(?:/(?<Endpoint>[^:@,/?=&]+)?)?" + // Endpoint (Group 5)
                "(?:\\?(?<Options>.+)?)?" + // Options (Group 6)
                "\\s*$"; // Optional whitespace padding at the end of the line

            if (!databaseUrl.Contains("://"))
            {
                throw new ArgumentException($"No scheme found in URI {databaseUrl}", nameof(databaseUrl));
            }

            var match = Regex.Match(databaseUrl, pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            if (!match.Success)
            {
                throw new ArgumentException($"{databaseUrl} is not a valid database URL", nameof(databaseUrl));
            }

            var queryString = HttpUtility.ParseQueryString(match.Groups["Options"].Value);
            var options = queryString.AllKeys.ToDictionary(k => k, k => queryString[k]);
            var hosts = new List<DatabaseHost>();

            if (options.TryGetValue("host", out var hostValue))
            {
                // Parse hosts from query string
                var hostValues = hostValue.Split(',');
                var portValues = options.TryGetValue("port", out var portValue) ? portValue.Split(',') : new string[0];

                if (portValues.Length > 1 && hostValues.Length != portValues.Length)
                {
                    throw new ArgumentException($"The number of ports doesn't match the number of hosts in URI {databaseUrl}", nameof(databaseUrl));
                }

                for (var i = 0; i < hostValues.Length; i++)
                {
                    var port = portValues.Length <= 1 ? portValues.ElementAtOrDefault(0) : portValues.ElementAt(i);
                    hosts.Add(new DatabaseHost
                    {
                        Host = hostValues[i],
                        Port = port != null && int.TryParse(port, out var p) ? p : (int?)null,
                    });
                }
            }
            else
            {
                var hostPortPattern = new Regex(@"^(?<Host>(?:\[[\da-f:]+\])|(?:\d{1,3}\.){3}\d{1,3}|[^:]+)(?::(?<Port>\d+))?$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

                // Parse hosts from URI host
                foreach (var host in match.Groups["Hosts"].Value.Split(','))
                {
                    if (string.IsNullOrEmpty(host))
                    {
                        hosts.Add(new DatabaseHost());
                        continue;
                    }

                    var hostMatch = hostPortPattern.Match(host);
                    if (!hostMatch.Success)
                    {
                        throw new ArgumentException($"The host {hostMatch.Value} is not a valid host", nameof(databaseUrl));
                    }

                    hosts.Add(new DatabaseHost
                    {
                        Host = HttpUtility.UrlDecode(hostMatch.Groups["Host"].Value),
                        Port = hostMatch.Groups["Port"].Success && int.TryParse(hostMatch.Groups["Port"].Value, out var port) ? port : (int?)null,
                    });
                }
            }

            var scheme = match.Groups["Scheme"].Value;
            var user = options.TryGetValue("user", out var userValue) ? userValue : HttpUtility.UrlDecode(match.Groups["Username"].Value);
            var password = options.TryGetValue("password", out var passwordValue) ? passwordValue : HttpUtility.UrlDecode(match.Groups["Password"].Value);
            var dbname = options.TryGetValue("dbname", out var dbValue) ? dbValue : HttpUtility.UrlDecode(match.Groups["Endpoint"].Value);

            var url = new DatabaseUrl
            {
                OriginalString = databaseUrl,
                Scheme = scheme,
                Username = user,
                Password = password,
                Options = options,
                Database = dbname,
                Hosts = hosts,
            };

            return url;
        }
    }
}
