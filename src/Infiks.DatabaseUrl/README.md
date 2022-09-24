# Infiks.DatabaseUrl

Library for translating database connection URIs into connection strings.

For example, the URI `postgres://user:secret@localhost:5432/mydb` is translated into
the connection string `Host=localhost:5432;Username=user;Password=secret;Database=mydb`.

## Installation

```
dotnet add package Infiks.DatabaseUrl
```

## Usage

Use the static `DatabaseUrlConverter.Convert()` method to convert a database URI into a connection string.

```csharp
var databaseUri = "postgres://user:secret@localhost:5432/mydb";
var connectionString = DatabaseUrlConverter.Convert(databaseUri);
// Result: Host=localhost:5432;Username=user;Password=secret;Database=mydb
```
