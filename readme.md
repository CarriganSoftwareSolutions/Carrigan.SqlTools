# Carrigan.SqlTools

Carrigan.SqlTools is a multi-dialect .NET SQL toolkit for generating SQL, building query expressions, executing generated queries, mapping rows to objects, and supporting property-level encryption/decryption workflows.

The project is organized around a shared core plus dialect-specific generator and client packages. Most users should install one of the dialect convenience packages:

- [Carrigan.SqlTools.PostgreSql](CarriganSqlTools/Carrigan.SqlTools.PostgreSql/readme.md)
- [Carrigan.SqlTools.SqlServer](CarriganSqlTools/Carrigan.SqlTools.SqlServer/readme.md)

Each convenience package references the matching generator and client packages so a single NuGet install gives you SQL generation plus execution helpers for that dialect.

---

## Table of Contents

- [Features](#features)
- [Installation](#installation)
- [Documentation](#documentation)
- [Package Organization](#package-organization)
- [Dialect Differences](#dialect-differences)
- [Safety Notes](#safety-notes)
- [License](#license)

---

## Features

- **Automatic CRUD generation**  
  Generate `SELECT`, `INSERT`, `UPDATE`, and `DELETE` statements from model types and attributes.

- **Dialect-specific SQL rendering**  
  SQL Server and PostgreSQL have different identifier quoting, parameter styles, paging syntax, identity-return syntax, and joined update/delete syntax. Each dialect package renders SQL using that database's conventions.

- **Manual query builders**  
  Build more advanced queries with builders for `SELECT`, `INSERT`, `UPDATE`, and `DELETE`, plus predicates, joins, `ORDER BY`, and paging helpers.

- **Client execution helpers**  
  Client packages wrap ADO.NET provider APIs for async and non-async execution, scalar queries, reader queries, object materialization, and encrypted-property decryption.

- **Carrigan.Core integration**  
  Shared interfaces and attributes support property-level encryption workflows and reusable helper behavior.

- **Analyzer support**  
  Dialect generator packages include Roslyn analyzers to catch invalid or risky SQL type attribute usage.

[Table of Contents](#table-of-contents)

---

## Installation

Install the dialect convenience package when you want both SQL generation and execution helpers.

```powershell
dotnet add package Carrigan.SqlTools.PostgreSql
```

```powershell
dotnet add package Carrigan.SqlTools.SqlServer
```

Install the generator and client packages separately when you only need part of a dialect.

```powershell
dotnet add package Carrigan.SqlTools.Generators.PostgreSql
dotnet add package Carrigan.SqlTools.Clients.PostgreSql
```

```powershell
dotnet add package Carrigan.SqlTools.Generators.SqlServer
dotnet add package Carrigan.SqlTools.Clients.SqlServer
```

The shared `Carrigan.SqlTools` and `Carrigan.SqlTools.Clients.Core` packages are normally installed transitively by the dialect packages.

[Table of Contents](#table-of-contents)

---

## Documentation

| Dialect | Convenience Package | Generator Package | Client Package | Full Documentation |
|---------|---------------------|-------------------|----------------|--------------------|
| PostgreSQL | `Carrigan.SqlTools.PostgreSql` | `Carrigan.SqlTools.Generators.PostgreSql` | `Carrigan.SqlTools.Clients.PostgreSql` | [PostgreSQL README](CarriganSqlTools/Carrigan.SqlTools.PostgreSql/readme.md) |
| SQL Server | `Carrigan.SqlTools.SqlServer` | `Carrigan.SqlTools.Generators.SqlServer` | `Carrigan.SqlTools.Clients.SqlServer` | [SQL Server README](CarriganSqlTools/Carrigan.SqlTools.SqlServer/readme.md) |

Useful direct links:

- [PostgreSQL getting started examples](CarriganSqlTools/Carrigan.SqlTools.PostgreSql/readme.md#getting-started-examples)
- [PostgreSQL data type mappings](CarriganSqlTools/Carrigan.SqlTools.PostgreSql/readme.md#data-type-mappings)
- [PostgreSQL Simple ADO.NET example](CarriganSqlTools/Carrigan.SqlTools.PostgreSql/readme.md#simple-ado.net-example-with-sqlquery)
- [SQL Server getting started examples](CarriganSqlTools/Carrigan.SqlTools.SqlServer/readme.md#getting-started-examples)
- [SQL Server data type mappings](CarriganSqlTools/Carrigan.SqlTools.SqlServer/readme.md#data-type-mappings)
- [SQL Server Simple ADO.NET example](CarriganSqlTools/Carrigan.SqlTools.SqlServer/readme.md#simple-ado.net-example-with-sqlquery)

[Table of Contents](#table-of-contents)

---

## Package Organization

| Package | Purpose |
|---------|---------|
| `Carrigan.SqlTools` | Shared SQL fragments, predicates, builders, tags, reflection helpers, attributes, and common abstractions. |
| `Carrigan.SqlTools.Generators.PostgreSql` | PostgreSQL SQL generation and PostgreSQL analyzer support. |
| `Carrigan.SqlTools.Generators.SqlServer` | SQL Server SQL generation and SQL Server analyzer support. |
| `Carrigan.SqlTools.Clients.Core` | Shared client-side execution, materialization, and decryption infrastructure used by provider-specific clients. |
| `Carrigan.SqlTools.Clients.PostgreSql` | PostgreSQL execution helpers built on Npgsql. |
| `Carrigan.SqlTools.Clients.SqlServer` | SQL Server execution helpers built on Microsoft.Data.SqlClient. |
| `Carrigan.SqlTools.PostgreSql` | Convenience package that references the PostgreSQL generator and client packages. |
| `Carrigan.SqlTools.SqlServer` | Convenience package that references the SQL Server generator and client packages. |

[Table of Contents](#table-of-contents)

---

## Dialect Differences

Carrigan.SqlTools keeps the public query-building model similar across dialects, but the generated SQL is intentionally different where the databases differ.

| Area | PostgreSQL | SQL Server |
|------|------------|------------|
| Identifier quoting | `"Customer"` | `[Customer]` |
| Parameters | `$1`, `$2`, `$3` | `@Name_1`, `@Id_2` |
| Insert generated key return | `RETURNING "Id"` | `OUTPUT INSERTED.[Id] INTO @OutputTable` |
| Joined update | `UPDATE target SET ... FROM source WHERE ...` | `UPDATE target SET ... FROM target JOIN source ON ... WHERE ...` |
| Joined delete | `DELETE FROM target USING source WHERE ...` | `DELETE target FROM target JOIN source ON ... WHERE ...` |
| Paging | `LIMIT/OFFSET` | `OFFSET/FETCH NEXT` |
| Case-insensitive LIKE | `ILIKE` when requested | Depends on collation / SQL Server behavior |

See the dialect READMEs for complete examples and type mapping details.

[Table of Contents](#table-of-contents)

---

## Safety Notes

This library may generate or execute SQL. Review generated SQL before using it in production systems and always test against non-production databases first.

Use caution with schema, migration, and data-modifying operations. The authors are not responsible for data loss, downtime, or unintended database changes.

Carrigan.SqlTools is not affiliated with or endorsed by Microsoft, the PostgreSQL project, Npgsql, or Microsoft.Data.SqlClient.

[Table of Contents](#table-of-contents)

---

## License

Carrigan.SqlTools  
Copyright © 2025 Carrigan Software Solutions LLC

Licensed under the Apache License, Version 2.0: http://www.apache.org/licenses/LICENSE-2.0

[Table of Contents](#table-of-contents)
