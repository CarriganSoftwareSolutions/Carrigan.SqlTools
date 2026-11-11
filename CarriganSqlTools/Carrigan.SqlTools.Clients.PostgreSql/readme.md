<a id="carrigan.sqltools.clients.postgresql"></a>

# Carrigan.SqlTools.Clients.PostgreSql

PostgreSQL client support for Carrigan.SqlTools.

This package provides the PostgreSQL-specific client layer for Carrigan.SqlTools. It is intended for applications and libraries that need to execute Carrigan.SqlTools-generated SQL against PostgreSQL databases.

## .NET compatibility and support

**Target frameworks:** `net10.0` and `net11.0`. This PostgreSQL package requires **.NET 10 or later** and does not provide .NET 9 binaries.

Carrigan Software Solutions LLC officially supports only .NET versions currently supported by Microsoft. Microsoft ended support for .NET 9 on November 10, 2026. Any binaries targeting obsolete .NET versions that remain available in other Carrigan packages are provided **as-is and without official support**; framework-specific testing, bug fixes, security updates, and technical support are not guaranteed. Framework targets may change in future releases.

---

## Installation

```bash
dotnet add package Carrigan.SqlTools.Clients.PostgreSql
```

## Package role

`Carrigan.SqlTools.Clients.PostgreSql` builds on the shared Carrigan.SqlTools client infrastructure and provides PostgreSQL-specific behavior for the client layer.

The intended package layering is:

```text
Carrigan.SqlTools
        |
        v
Carrigan.SqlTools.Clients.Core
        |
        v
Carrigan.SqlTools.Clients.PostgreSql
```

Most application projects that need PostgreSQL client support should reference this package directly. The shared core client package is intended to be brought in transitively.

## When to use this package

Use this package when your project needs PostgreSQL support for Carrigan.SqlTools client operations.

This package is appropriate for:

- PostgreSQL-backed applications using Carrigan.SqlTools.
- Libraries that need Carrigan.SqlTools PostgreSQL client support.
- Tests that execute generated SQL against PostgreSQL.
- Projects that need PostgreSQL-specific database client behavior.

## Related packages

- `Carrigan.SqlTools`
- `Carrigan.SqlTools.Clients.Core`
- `Carrigan.SqlTools.Generators.PostgreSql`
- `Carrigan.SqlTools.Clients.SqlServer`
- `Carrigan.SqlTools.Generators.SqlServer`

## Versioning

Carrigan.SqlTools packages are currently prerelease packages while the public API is still being refined. Breaking changes may occur between prerelease versions.

## License

This project is licensed under the Apache License 2.0.

See the included `LICENSE` file for details.
