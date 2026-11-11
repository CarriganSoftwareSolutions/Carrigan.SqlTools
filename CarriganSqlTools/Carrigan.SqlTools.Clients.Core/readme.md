<a id="carrigan.sqltools.clients.core"></a>

# Carrigan.SqlTools.Clients.Core

Shared core client infrastructure for the Carrigan.SqlTools database dialect client packages.

This package contains the common base types, abstractions, and shared client-side functionality used by database-specific Carrigan.SqlTools client implementations. It is primarily intended to be consumed **transitively** through dialect-specific packages.

Most users should install one of the database-specific client packages instead of referencing this package directly.

## Intended usage

Use this package directly only when you are:

- Building a new Carrigan.SqlTools client package for another SQL dialect.
- Extending the Carrigan.SqlTools client infrastructure.
- Working on the Carrigan.SqlTools internals.
- Creating tests or shared infrastructure for dialect-specific clients.

For normal application development, prefer installing a dialect-specific package, such as:

```bash
dotnet add package Carrigan.SqlTools.Clients.SqlServer
```

or:

```bash
dotnet add package Carrigan.SqlTools.Clients.PostgreSql
```

Those packages should bring this package in automatically as a dependency.

## .NET compatibility and support

**Target frameworks:** `net9.0`, `net10.0`, and `net11.0`.

**Microsoft ended support for .NET 9 on November 10, 2026.** We continue to provide .NET 9 binaries for compatibility with existing applications. However, Carrigan Software Solutions LLC officially supports only .NET versions currently supported by Microsoft. Binaries targeting unsupported .NET versions are provided **as-is and without official support**; framework-specific testing, bug fixes, security updates, and technical support are not guaranteed. Framework targets may be removed in future releases. For production use, upgrade to a supported .NET version.

---

## Package role

`Carrigan.SqlTools.Clients.Core` provides shared functionality for the client layer of Carrigan.SqlTools, while database-specific behavior belongs in the dialect client packages.

The intended package layering is:

```text
Carrigan.SqlTools
        |
        v
Carrigan.SqlTools.Clients.Core
        |
        +--> Carrigan.SqlTools.Clients.SqlServer
        |
        +--> Carrigan.SqlTools.Clients.PostgreSql
```

## What this package is not

This package is not intended to be the main public entry point for most applications.

It does not represent a complete SQL Server, PostgreSQL, or other database client by itself. Instead, it provides the shared foundation used by those client packages.

## Related packages

- `Carrigan.SqlTools`
- `Carrigan.SqlTools.Clients.SqlServer`
- `Carrigan.SqlTools.Clients.PostgreSql`
- `Carrigan.SqlTools.Generators.SqlServer`
- `Carrigan.SqlTools.Generators.PostgreSql`

## Versioning

Carrigan.SqlTools packages are currently prerelease packages while the public API is still being refined. Breaking changes may occur between prerelease versions.

## License

This project is licensed under the Apache License 2.0.

See the included `LICENSE` file for details.
