# Infiks.Configuration.DatabaseUrl

A configuration provider that translates database connection URIs into connection strings.

For example, Heroku provides the database URI in an environment variable `DATABASE_URL=postgres://user:secret@localhost:5432/mydb`.
The translated connection string is `Host=localhost:5432;Username=user;Password=secret;Database=mydb`.

## Installation

```
dotnet add package Infiks.Configuration.DatabaseUrl
```

## Usage

Use the `AddDatabaseUrl()` extension method to enable database URI translation. By default, the environment variable that is used is `DATABASE_URL` and the connection string name is `DatabaseUrl`. These settings are configurable, for example `AddDatabaseUrl(environmentVariable: "DB_URL", connectionString: "Default")`.

```csharp
IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((_, configuration) =>
    {
        configuration.AddDatabaseUrl();
    })
    .ConfigureServices((context, services) =>
    {
        ...
    })
    .Build();
```
