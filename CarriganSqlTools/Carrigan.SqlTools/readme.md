<a id="carrigan.sqltools"></a>

# Carrigan.SqlTools
<!--Ignore Spelling: dotnet sqlquery adonet decrypters encryptors-->

Carrigan.SqlTools contains the shared SQL modeling, reflection, predicate, join, paging, and query-building abstractions used by the Carrigan.SqlTools dialect generator and client packages.

This package is the core foundation for dialect-specific generators such as **Carrigan.SqlTools.Generators.SqlServer** and **Carrigan.SqlTools.Generators.PostgreSql**. Most applications should reference a dialect generator package directly instead of using this package by itself.

This package may be installed directly when you are building shared Carrigan.SqlTools infrastructure, writing a new dialect package, or using the common model types in tests or libraries.

This library may be used to model or generate SQL through dialect packages. Review generated SQL before use in production systems. Always test against non-production databases first.

Use caution with schema, migration, and data-modifying operations. The authors are not responsible for data loss, downtime, or unintended database changes.

---

## Installation

```powershell
dotnet add package Carrigan.SqlTools
```

For SQL Server generation, install:

```powershell
dotnet add package Carrigan.SqlTools.Generators.SqlServer
```

For PostgreSQL generation, install:

```powershell
dotnet add package Carrigan.SqlTools.Generators.PostgreSql
```

For SQL Server execution helpers, install:

```powershell
dotnet add package Carrigan.SqlTools.Clients.SqlServer
```

For PostgreSQL execution helpers, install:

```powershell
dotnet add package Carrigan.SqlTools.Clients.PostgreSql
```

---

## Package role

`Carrigan.SqlTools` provides shared infrastructure, including:

- SQL fragment types and query containers.
- Table, column, alias, parameter, procedure, and result-column identifier wrappers.
- Reflection caches used to map C# model classes and properties to SQL tables, columns, and parameters.
- Predicate, join, order-by, paging, and query-builder base types.
- Common attributes such as `IdentifierAttribute`, `PrimaryKeyAttribute`, `ParameterAttribute`, `AliasAttribute`, and `SelectTagAttribute`.
- Shared exception types used by the generator packages.

Dialect-specific behavior, such as identifier quoting, parameter rendering, data type mapping, `RETURNING`, `OUTPUT`, `LIMIT/OFFSET`, or `OFFSET/FETCH NEXT`, belongs in the dialect generator packages.

---

## Related packages

- `Carrigan.SqlTools.Generators.SqlServer`
- `Carrigan.SqlTools.Generators.PostgreSql`
- `Carrigan.SqlTools.Clients.Core`
- `Carrigan.SqlTools.Clients.SqlServer`
- `Carrigan.SqlTools.Clients.PostgreSql`

---

## License

Carrigan.SqlTools  
Copyright © 2025 Carrigan Software Solutions LLC

Licensed under the Apache License, Version 2.0.
