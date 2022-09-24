# Infiks DatabaseUrl

Packages for translating database connection URIs into connection strings.

For example, the URI `postgres://user:secret@localhost:5432/mydb` is translated into
the connection string `Host=localhost:5432;Username=user;Password=secret;Database=mydb`.

This repository contains two packages:

- [Infiks.Configuration](./src/Infiks.Configuration/README.md)
- [Infiks.Configuration.DatabaseUrl](./src/Infiks.Configuration.DatabaseUrl/README.md)
