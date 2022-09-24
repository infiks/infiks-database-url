namespace Infiks.DatabaseUrl
{
    /// <summary>
    /// Database host info.
    /// </summary>
    public class DatabaseHost
    {
        /// <summary>
        /// The database connection host name or IP address.
        /// </summary>
        public string Host { get; set; } = string.Empty;

        /// <summary>
        /// The database connection port number or <see langword="null"/> for the default port.
        /// </summary>
        public int? Port { get; set; }
    }
}
