using System;
using Xunit;

namespace Infiks.DatabaseUrl.Tests
{
    public class TestDatabaseUrlConverter
    {
        [Theory]
        [InlineData("db:/host")]
        [InlineData("db//host/db?k=v")]
        [InlineData("://user:password@host:1234/db?k=v")]
        [InlineData("db://localhost/db?host=host1,host2,host3&port=123,456")]
        [InlineData("db://localhost/db?host=host1&port=123,456")]
        public void TestParseInvalid(string databaseUrl)
        {
            Assert.Throws<ArgumentException>(() => DatabaseUrlConverter.Parse(databaseUrl));
        }

        [Theory]
        [InlineData("db://", "db", "", "", "", null, "", 0)]
        [InlineData("db:///mydb", "db", "", "", "", null, "mydb", 0)]
        [InlineData("db://localhost", "db", "", "", "localhost", null, "", 0)]
        [InlineData("db://localhost:1234", "db", "", "", "localhost", 1234, "", 0)]
        [InlineData("db://localhost:1234/mydb", "db", "", "", "localhost", 1234, "mydb", 0)]
        [InlineData("db://localhost/mydb", "db", "", "", "localhost", null, "mydb", 0)]
        [InlineData("db://user@localhost", "db", "user", "", "localhost", null, "", 0)]
        [InlineData("db://user:secret@localhost", "db", "user", "secret", "localhost", null, "", 0)]
        [InlineData("db:///mydb?host=localhost", "db", "", "", "localhost", null, "mydb", 1)]
        [InlineData("db:///mydb?host=localhost&port=1234", "db", "", "", "localhost", 1234, "mydb", 2)]
        [InlineData("postgres://john:secret@dbserver.com:4568/testdb", "postgres", "john", "secret", "dbserver.com", 4568, "testdb", 0)]
        [InlineData("sqlite://mike:qwerty@localhost/appdb?param=value", "sqlite", "mike", "qwerty", "localhost", null, "appdb", 1)]
        [InlineData("mongodb://alex@dbserver.com/testdb?param=value&more=val", "mongodb", "alex", "", "dbserver.com", null, "testdb", 2)]
        public void TestParse(string databaseUrl, string scheme, string username, string password, string host, int? port, string database, int optionCount)
        {
            var url = DatabaseUrlConverter.Parse(databaseUrl);
            Assert.Equal(1, url.Hosts.Count);
            Assert.Equal(databaseUrl, url.OriginalString);
            Assert.Equal(scheme, url.Scheme);
            Assert.Equal(username, url.Username);
            Assert.Equal(password, url.Password);
            Assert.Equal(host, url.Hosts[0].Host);
            Assert.Equal(port, url.Hosts[0].Port);
            Assert.Equal(database, url.Database);
            Assert.Equal(optionCount, url.Options.Count);
        }

        [Theory]
        [InlineData("db://host1:123/somedb", new[] { "host1" }, new[] { 123 })]
        [InlineData("db://host1:123,host2/somedb", new[] { "host1", "host2" }, new[] { 123, -1 })]
        [InlineData("db://user:secret@host1:123,host2:456/somedb", new[] { "host1", "host2" }, new[] { 123, 456 })]
        [InlineData("db://user@host1:123,host2:456,host3:789/somedb?param=value", new[] { "host1", "host2", "host3" }, new[] { 123, 456, 789 })]
        [InlineData("db:///somedb?host=host1,host2,host3&port=123,456,789", new[] { "host1", "host2", "host3" }, new[] { 123, 456, 789 })]
        [InlineData("db:///somedb?host=host1,host2,host3", new[] { "host1", "host2", "host3" }, new[] { -1, -1, -1 })]
        [InlineData("db:///somedb?host=host1,host2,host3&port=123", new[] { "host1", "host2", "host3" }, new[] { 123, 123, 123 })]
        public void TestParseMultipleHosts(string databaseUrl, string[] hosts, int[] ports)
        {
            var url = DatabaseUrlConverter.Parse(databaseUrl);
            Assert.Equal(hosts.Length, url.Hosts.Count);
            Assert.Equal(ports.Length, url.Hosts.Count);

            for (var i = 0; i < url.Hosts.Count; i++)
            {
                var expectedHost = hosts[i];
                var expectedPort = ports[i] == -1 ? (int?)null : ports[i];
                Assert.Equal(expectedHost, url.Hosts[i].Host);
                Assert.Equal(expectedPort, url.Hosts[i].Port);
            }
        }

        [Theory]
        [InlineData("db://localhost/somedb?user=john", "john", "", "somedb")]
        [InlineData("db://john@localhost/somedb?password=secret", "john", "secret", "somedb")]
        [InlineData("db://localhost/somedb?user=john&password=secret", "john", "secret", "somedb")]
        [InlineData("db://localhost?user=john&password=secret&dbname=somedb", "john", "secret", "somedb")]
        public void TestParseDbParameters(string databaseUrl, string username, string password, string database)
        {
            var url = DatabaseUrlConverter.Parse(databaseUrl);
            Assert.Equal(1, url.Hosts.Count);
            Assert.Equal(username, url.Username);
            Assert.Equal(password, url.Password);
            Assert.Equal(database, url.Database);
        }

        [Theory]
        [InlineData("db://[2001:db8::1234]/database", "[2001:db8::1234]", null, "database")]
        [InlineData("db://[2001:DB8::1234]:5000/database", "[2001:DB8::1234]", 5000, "database")]
        [InlineData("postgresql:///dbname?host=/var/lib/postgresql", "/var/lib/postgresql", null, "dbname")]
        [InlineData("postgresql://%2Fvar%2Flib%2Fpostgresql/dbname", "/var/lib/postgresql", null, "dbname")]
        public void TestParseSpecial(string databaseUrl, string host, int? port, string database)
        {
            var url = DatabaseUrlConverter.Parse(databaseUrl);
            Assert.Equal(1, url.Hosts.Count);
            Assert.Equal(host, url.Hosts[0].Host);
            Assert.Equal(port, url.Hosts[0].Port);
            Assert.Equal(database, url.Database);
        }

        [Theory]
        [InlineData("db://user@localhost:5433/mydb?a=1&b=2&c=3", 3, new[] { "a", "b", "c" }, new[] { "1", "2", "3" })]
        [InlineData("postgresql://user@localhost:5433/mydb?options=-c%20synchronous_commit%3Doff", 1, new[] { "options" }, new[] { "-c synchronous_commit=off" })]
        public void TestParseOptions(string databaseUrl, int optionCount, string[] keys, string[] values)
        {
            var url = DatabaseUrlConverter.Parse(databaseUrl);
            Assert.Equal(optionCount, keys.Length);
            Assert.Equal(optionCount, values.Length);
            Assert.Equal(optionCount, url.Options.Count);

            for (var i = 0; i < keys.Length; i++)
            {
                var key = keys[i];
                Assert.True(url.Options.ContainsKey(key));
                Assert.Equal(values[i], url.Options[key]);
            }
        }

        [Theory]
        [InlineData("db://", "")]
        [InlineData("db://localhost", "Host=localhost")]
        [InlineData("db://user@localhost", "Host=localhost;Username=user")]
        [InlineData("db://user@localhost:1234/mydb?a=1&b=2&c=3", "Host=localhost:1234;Username=user;Database=mydb;a=1;b=2;c=3")]
        public void TestConvertGeneric(string databaseUrl, string connectionString)
        {
            var result = DatabaseUrlConverter.Convert(databaseUrl);
            Assert.Equal(connectionString, result);
        }

        [Theory]
        [InlineData("postgres://user@localhost:5433/mydb?application_name=Test", "Host=localhost:5433;Username=user;Database=mydb;Application Name=Test")]
        [InlineData("postgresql://user@localhost:5433/mydb?application_name=Test", "Host=localhost:5433;Username=user;Database=mydb;Application Name=Test")]
        public void TestConvertNpgsql(string databaseUrl, string connectionString)
        {
            var result = DatabaseUrlConverter.Convert(databaseUrl);
            Assert.Equal(connectionString, result);
        }
    }
}
