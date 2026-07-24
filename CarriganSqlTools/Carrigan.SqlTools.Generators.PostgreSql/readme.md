<a id="carrigan.sqltools.generators.postgresql"></a>

# Carrigan.SqlTools.Generators.PostgreSql

Carrigan.SqlTools.Generators.PostgreSql is a .NET library that adds PostgreSQL-specific SQL generation to Carrigan.SqlTools while still giving you control when you need it.

It automatically generates PostgreSQL `SELECT`, `INSERT`, `UPDATE`, and `DELETE` statements using reflection. The PostgreSQL generator also provides a safe, object-oriented API for building more advanced PostgreSQL queries.

A companion package, **Carrigan.SqlTools.Clients.PostgreSql**, provides the PostgreSQL client layer for executing generated queries, mapping rows to objects, and handling decryption of encrypted properties.

The transitive dependency **Carrigan.Core** provides interfaces and property attributes used for custom property-level encryption and other shared helper behavior.

This package includes PostgreSQL-focused Roslyn analyzers bundled with the package to assist with safe usage patterns.

This project is not affiliated with or endorsed by the PostgreSQL project.

This library may generate or execute SQL. Review generated SQL before use in production systems. Always test against non-production databases first.

Use caution with schema, migration, and data-modifying operations. The authors are not responsible for data loss, downtime, or unintended database changes.

---

## Table of Contents
- [Features](#features)
- [Installation](#installation)
- [Getting Started Examples](#getting-started-examples)
  - [Select All Rows](#select-all-rows)
  - [Select by Id](#select-by-id)
  - [Insert](#insert)
  - [Insert with Auto Id](#insert-with-auto-id)
  - [Update by Id](#update-by-id)
  - [Update by Id (selected columns)](#update-by-id-selected-columns)
  - [Delete](#delete)
  - [Delete by Id (multiple keys)](#delete-by-id-multiple-keys)
- [More Complex Examples](#more-complex-examples)
  - [Select with Joins and Order By](#select-with-joins-and-order-by)
  - [Select with Two Part Order By](#select-with-two-part-order-by)
  - [Delete with Using and Where](#delete-with-using-and-where)
  - [Select Count With Where](#select-count-with-where)
  - [Update with From and Where](#update-with-from-and-where)
  - [Aggregate Expression Examples](#aggregate-expression-examples)
- [Attribute Examples](#attribute-examples)
  - [Table, Column and Key](#table-column-and-key)
  - [Identifier and Primary Key](#identifier-and-primary-key)
  - [Procedure and Parameter](#procedure-and-parameter)
- [Running Queries (Async & Non-Async)](#running-queries-async--non-async)
  - [Async: ExecuteNonQueryAsync / ExecuteScalarAsync / ExecuteReaderAsync\<T>](#async-executenonqueryasync--executescalarasync--executereaderasynct)
  - [Non-Async: ExecuteNonQuery / ExecuteScalar / ExecuteReader\<T>](#non-async-executenonquery--executescalar--executereadert)
- [Simple ADO.NET Example With SqlQuery](#simple-ado.net-example-with-sqlquery)
- [Data Type Mappings](#data-type-mappings)
  - [Default PostgreSQL Types](#default-postgresql-types)
  - [Supported CLR Types](#supported-clr-types)
  - [PostgreSQL Type Override Attributes](#postgresql-type-override-attributes)
  - [Analyzer Warnings](#analyzer-warnings)
  - [Arrays](#arrays)
- [ExampleEncryptor (AesGcm-based) and IDecrypters](#exampleencryptor-aesgcm-based-and-idecrypters)
- [License](#license)

---

## Features

- **Automatic PostgreSQL CRUD generation**
  Build PostgreSQL `SELECT`, `INSERT`, `UPDATE`, and `DELETE` queries with reflection.

- **PostgreSQL dialect rendering**
  Generates double-quoted identifiers, native positional parameters such as `$1`, `RETURNING` for insert return values, PostgreSQL `UPDATE ... FROM`, PostgreSQL `DELETE ... USING`, and PostgreSQL `LIMIT/OFFSET` paging.

- **Carrigan.Core integration**
  Interfaces and property-level attributes enable property-level encryption and decryption.

- **Manual query builder**
  Safely construct advanced SQL with `JOIN`, predicates such as `Equal`, `GreaterThan`, `IsNull`, `Like`, `And`, `Or`, `Xor`, and `Not`, `ORDER BY`, and PostgreSQL pagination through `LimitOffset`.

- **PostgreSQL-specific predicate behavior**
  `LIKE` remains case-sensitive by default. Case-insensitive matching uses PostgreSQL `ILIKE` when requested.

- **Dictionary → object mapping support**
  Use the invocation system to populate typed models from database rows.

- **Focused on PostgreSQL**
  SQL output targets PostgreSQL syntax and PostgreSQL type names.

- **Optional execution helpers**
  Use `Carrigan.SqlTools.Clients.PostgreSql` for async and non-async helpers that run generated queries through Npgsql.

[Table of Contents](#table-of-contents)

---

## Installation

Install the PostgreSQL generator package:

```powershell
dotnet add package Carrigan.SqlTools.Generators.PostgreSql
```

For PostgreSQL execution helpers:

```powershell
dotnet add package Carrigan.SqlTools.Clients.PostgreSql
```

[Table of Contents](#table-of-contents)

---

## Getting Started Examples

Use `Carrigan.SqlTools.PostgreSql.SqlGenerator<T>` to produce a **`SqlQuery`** with `QueryText`, `CommandType`, and `Parameters`.

All examples use the following `using` statements to keep code clean.

```csharp
using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Base.Tests.Helpers;
using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.OrderByClause;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.PostgreSql;
using Carrigan.SqlTools.Sets;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tags;
using System.Text;

// Example data models
public class Customer
{
    [PrimaryKey] // note: PrimaryKey takes precedence over Key for the SQL generator.
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public string Phone { get; set; } = "";
}

public class Order
{
    [PrimaryKey] // Required for key-based SQL generation methods.
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public int PaymentMethodId { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal Total { get; set; }
}

// Generators
public SqlGenerator<Customer> customerGenerator = new();
public SqlGenerator<Order> orderGenerator = new();
```

[Table of Contents](#table-of-contents)

### Select All Rows

```csharp
SqlQuery query = customerGenerator.SelectAll();
// SELECT "Customer".* FROM "Customer"
```

[Table of Contents](#table-of-contents)

### Select by Id

Key attribute required, and composite keys are supported by specifying multiple keys.

```csharp
Customer entity = new() { Id = 42 };
SqlQuery query = customerGenerator.SelectById(entity);

// SELECT "Customer".*
// FROM "Customer"
// WHERE ("Customer"."Id" = $1)
```

[Table of Contents](#table-of-contents)

### Insert

```csharp
Customer entity = new()
{
    Id = 42,
    Name = "Hank",
    Email = "Hank@example.com",
    Phone = "+1(555)555-5555"
};

InsertBuilder<Customer> insertBuilder = new()
{
    Records = [entity]
};

SqlQuery query = customerGenerator.Insert(insertBuilder);

// INSERT INTO "Customer" ("Id", "Name", "Email", "Phone")
// VALUES ($1, $2, $3, $4);
```

[Table of Contents](#table-of-contents)

### Insert with Auto Id

Key attribute required, and Id columns must have a default value.

```csharp
Customer entity = new()
{
    Name = "Hank",
    Email = "Hank@example.com",
    Phone = "+1(555)555-5555"
};

SqlQuery query = customerGenerator.InsertAutoId(entity);

// INSERT INTO "Customer" ("Name", "Email", "Phone")
// VALUES ($1, $2, $3)
// RETURNING "Id";
```

[Table of Contents](#table-of-contents)

### Update by Id

Key attribute required, and composite keys are supported by specifying multiple keys.

```csharp
Customer entity = new()
{
    Id = 42,
    Name = "Hank",
    Email = "Hank@example.com",
    Phone = "+1(555)555-5555"
};

SqlQuery query = customerGenerator.UpdateById(entity);

// UPDATE "Customer"
// SET "Name" = $1, "Email" = $2, "Phone" = $3
// WHERE "Id" = $4;
```

[Table of Contents](#table-of-contents)

<a id="update-by-id-selected-columns"></a>

### Update by Id (selected columns)

Key attribute required, and composite keys are supported by specifying multiple keys.

`ColumnCollection<T>` validates property names and throws an error when a property name is not valid for the model.

```csharp
ColumnCollection<Customer> columns = new(nameof(Customer.Email));
Customer entity = new() { Id = 42, Name = "Hank", Email = "Hank@example.gov" };
SqlQuery query = customerGenerator.UpdateById(entity, columns);

// UPDATE "Customer"
// SET "Email" = $1
// WHERE "Id" = $2;
```

[Table of Contents](#table-of-contents)

### Delete

Key attribute required, and composite keys are supported by specifying multiple keys.

```csharp
Customer entity = new() { Id = 42 };
SqlQuery query = customerGenerator.Delete(entity);

// DELETE FROM "Customer"
// WHERE ("Customer"."Id" = $1)
```

[Table of Contents](#table-of-contents)

<a id="delete-by-id-multiple-keys"></a>

### Delete by Id (multiple keys)

Key attribute required, and composite keys are supported by specifying multiple keys.

```csharp
Customer[] entities = [new() { Id = 1 }, new() { Id = 2 }];
SqlQuery query = customerGenerator.DeleteById(entities);

// DELETE FROM "Customer"
// WHERE (("Customer"."Id" = $1)
//    OR ("Customer"."Id" = $2))
```

[Table of Contents](#table-of-contents)

---

## More Complex Examples

Use `SqlGenerator<T>` to produce a **`SqlQuery`** with `QueryText`, `CommandType`, and `Parameters`.

All examples use the following `using` statements to keep code clean.

```csharp
using Carrigan.SqlTools.Base.Tests.Helpers;
using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.OrderByClause;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.PostgreSql;
using Carrigan.SqlTools.Sets;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tags;

// Example data models
public class Customer
{
    [PrimaryKey]
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public string Phone { get; set; } = "";
}

public class Order
{
    [PrimaryKey]
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public int PaymentMethodId { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal Total { get; set; }
}

// Generators
public SqlGenerator<Customer> customerGenerator = new();
public SqlGenerator<Order> orderGenerator = new();
```

### Select with Joins and Order By

`ColumnEqualsColumn<LeftT, RightT>` validates property names and throws an error when a property name is not valid for the model.

`OrderBy<Order>` validates property names and throws an error when a property name is not valid for `Order`.

```csharp
ColumnEqualsColumn<Customer, Order> predicate = new(nameof(Customer.Id), nameof(Order.CustomerId));
InnerJoin<Order> join = new(predicate);

OrderBy<Order> orderByOrderDate = new(nameof(Order.OrderDate));

SelectBuilder<Customer> selectBuilder = new()
{
    Joins = join,
    OrderBys = orderByOrderDate
};

SqlQuery query = customerGenerator.Select(selectBuilder);

// SELECT "Customer".*
// FROM "Customer"
// INNER JOIN "Order"
//    ON ("Customer"."Id" = "Order"."CustomerId")
// ORDER BY "Order"."OrderDate" ASC
```

[Table of Contents](#table-of-contents)

### Select with Two Part Order By

`ColumnEqualsColumn<LeftT, RightT>` validates property names and throws an error when a property name is not valid for the model.

`OrderBy<T>` validates property names and throws an error when a property name is not valid for the specified model.

```csharp
ColumnEqualsColumn<Customer, Order> predicate = new(nameof(Customer.Id), nameof(Order.CustomerId));

InnerJoin<Order> join = new(predicate);

OrderBy<Order> orderByOrderDate = new(nameof(Order.OrderDate));
OrderBy<Customer> orderByCustomerId = new(nameof(Customer.Id), SortDirectionEnum.Descending);
OrderBys orderBys = new(orderByCustomerId, orderByOrderDate);

SelectBuilder<Customer> selectBuilder = new()
{
    Joins = join,
    OrderBys = orderBys
};

SqlQuery query = customerGenerator.Select(selectBuilder);

// SELECT "Customer".*
// FROM "Customer"
// INNER JOIN "Order"
//    ON ("Customer"."Id" = "Order"."CustomerId")
// ORDER BY "Customer"."Id" DESC,
//          "Order"."OrderDate" ASC
```

[Table of Contents](#table-of-contents)

### Delete with Using and Where

PostgreSQL joined deletes use `DELETE FROM target USING source ... WHERE ...`. The target table is not repeated as a joined `FROM` source.

`ColumnEqualsColumn<LeftT, RightT>` validates property names and throws an error when a property name is not valid for the model.

`ColumnValue<T>` validates property names and throws an error when a property name is not valid for the model.

```csharp
ColumnEqualsColumn<Customer, Order> predicate = new(nameof(Customer.Id), nameof(Order.CustomerId));
ColumnValue<Customer> customerEmail = new(nameof(Customer.Email), "spam@example.com");

DeleteBuilder<Order> deleteBuilder = new()
{
    Usings = [TableTag.Get<Customer>()],
    Where = new And(predicate, customerEmail)
};

SqlQuery query = orderGenerator.Delete(deleteBuilder);

// DELETE FROM "Order" USING "Customer"
// WHERE (("Customer"."Id" = "Order"."CustomerId")
//   AND ("Customer"."Email" = $1))
```

[Table of Contents](#table-of-contents)

### Select Count With Where

`Column<T>` validates property names and throws an error when a property name is not valid for the model.

```csharp
Column<Order> totalCol = new(nameof(Order.Total));
Parameter minTotal = new(500m, "Total");
GreaterThan greaterThan = new(totalCol, minTotal);

SqlQuery query = orderGenerator.SelectCount(null, null, null, greaterThan);

// SELECT COUNT("Order"."Id")
// FROM "Order"
// WHERE ("Order"."Total" > $1)
```

[Table of Contents](#table-of-contents)

### Update with From and Where

PostgreSQL joined updates use `UPDATE target SET ... FROM source ... WHERE ...`. The update target is not repeated in the `FROM` clause unless you are intentionally doing a self-join with an alias.

`ColumnCollection<T>` validates property names and throws an error when a property name is not valid for the model.

`ColumnEqualsColumn<LeftT, RightT>` validates property names and throws an error when a property name is not valid for the model.

`ColumnValue<T>` validates property names and throws an error when a property name is not valid for the model.

```csharp
Order entity = new() { Id = 10, Total = 123.45m };

ColumnCollection<Order> columnCollection = new(nameof(Order.Total));

ColumnEqualsColumn<Order, Customer> predicate = new(nameof(Order.CustomerId), nameof(Customer.Id));
ColumnValue<Customer> customerEmailEquals = new(nameof(Customer.Email), "spam@example.com");

UpdateBuilder<Order> updateBuilder = new()
{
    Values = entity,
    UpdateColumns = columnCollection,
    From = [TableTag.Get<Customer>()],
    Where = new And(predicate, customerEmailEquals)
};

SqlQuery query = orderGenerator.Update(updateBuilder);

// UPDATE "Order"
// SET "Total" = $1
// FROM "Customer"
// WHERE (("Order"."CustomerId" = "Customer"."Id")
//   AND ("Customer"."Email" = $2))
```

[Table of Contents](#table-of-contents)

### Aggregate Expression Examples

```csharp
Column<Grades> gradePoint = new(nameof(Grades.GradePoint));

SelectBuilder<Grades> selectBuilder = new()
{
    Selects = new SelectTags
    (
        SelectTagGenerator.Get<Grades>(nameof(Grades.StudentId)),
        SelectTagGenerator.Get<Grades>(nameof(Grades.CourseCode)),
        new SelectTag(new Average(gradePoint), "AverageGradePoint"),
        new SelectTag(new Sum(gradePoint), "TotalGradePoints"),
        new SelectTag(new Min(gradePoint), "MinimumGradePoint"),
        new SelectTag(new Max(gradePoint), "MaximumGradePoint"),
        new SelectTag(new Count(gradePoint), "GradePointCount")
    ),
    GroupBys = GroupBys
        .New<Grades>(nameof(Grades.StudentId))
        .Append<Grades>(nameof(Grades.CourseCode))
};

SqlQuery query = selectBuilder.AsSqlQuery();

//  SELECT 
//      "Grades"."StudentId", 
//      "Grades"."CourseCode", 
//      AVG("Grades"."GradePoint") AS "AverageGradePoint", 
//      SUM("Grades"."GradePoint") AS "TotalGradePoints",
//      MIN("Grades"."GradePoint") AS "MinimumGradePoint", 
//      MAX("Grades"."GradePoint") AS "MaximumGradePoint", 
//      COUNT("Grades"."GradePoint") AS "GradePointCount" 
//  FROM "Grades" 
//  GROUP BY 
//      "Grades"."StudentId",
//      "Grades"."CourseCode"
```

[Table of Contents](#table-of-contents)



---

## Attribute Examples

Use `SqlGenerator<T>` to produce a **`SqlQuery`** with `QueryText`, `CommandType`, and `Parameters`.

You can use the `[Table]` attribute from `System.ComponentModel.DataAnnotations.Schema` to override the table name. When no table attribute or `IdentifierAttribute` is present, the table name is assumed to be the same as the class name. You can also specify a schema name.

All examples use `using` statements to keep code clean.

```csharp
using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Base.Tests.Helpers;
using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.PostgreSql;
using Carrigan.SqlTools.SqlGenerators;

// Generators
public SqlGenerator<PhoneModel> phoneGenerator = new();
public SqlGenerator<EmailModel> emailGenerator = new();
public SqlGenerator<ProcedureExec> procedureExecGenerator = new();
```

<a id="table-column-and-key"></a>

### Table, Column and Key

```csharp
[Table("Phone", Schema="schema")]
internal class PhoneModel
{
    [Key]
    public int Id { get; set; }
    public int CustomerId { get; set; }
    [Column("Phone")]
    public string? PhoneNumber { get; set; }
}

PhoneModel phone = new()
{
    Id = 2718,
    CustomerId = 3141,
    PhoneNumber = "07700 900461"
};

SqlQuery query = phoneGenerator.UpdateById(phone);

// UPDATE "schema"."Phone"
// SET "CustomerId" = $1, "Phone" = $2
// WHERE "Id" = $3;
```

[Table of Contents](#table-of-contents)

### Identifier and Primary Key

```csharp
[Identifier("Email", "schema")]
internal class EmailModel
{
    [PrimaryKey]
    public int Id { get; set; }
    public int CustomerId { get; set; }
    [Identifier("Email")]
    public string? EmailAddress { get; set; }
}

EmailModel email = new()
{
    Id = 10,
    CustomerId = 313,
    EmailAddress = "Exterminate@GenericTinCanLand.gov"
};

SqlQuery query = emailGenerator.UpdateById(email);

// UPDATE "schema"."Email"
// SET "CustomerId" = $1, "Email" = $2
// WHERE "Id" = $3;
```

[Table of Contents](#table-of-contents)

### Procedure and Parameter

```csharp
[Identifier("UpdateThing", "schema")]
internal class ProcedureExec
{
    [Parameter("SomeValue")]
    public string? ValueColumn { get; set; }
}

ProcedureExec procedureExec = new()
{
    ValueColumn = "DangIt"
};

SqlQuery query = procedureExecGenerator.Procedure(procedureExec);

// "schema"."UpdateThing"
```

[Table of Contents](#table-of-contents)

---

<a id="running-queries-async--non-async"></a>

## Running Queries (Async & Non-Async)

The PostgreSQL execution helpers live in **Carrigan.SqlTools.Clients.PostgreSql** and use Npgsql.

```csharp
using System.Collections.Generic;
using Carrigan.Core.Interfaces;
using Carrigan.SqlTools.Clients.PostgreSql;
using Carrigan.SqlTools.PostgreSql;
using Carrigan.SqlTools.SqlGenerators;
using Npgsql;
```

<a id="async-executenonqueryasync--executescalarasync--executereaderasynct"></a>

### Async: `ExecuteNonQueryAsync` / `ExecuteScalarAsync` / `ExecuteReaderAsync<T>`

```csharp
// Build a query (example: SelectAll)
SqlQuery query = customerGenerator.SelectAll();

NpgsqlConnection connection = new(<your connection string>);
NpgsqlTransaction? transaction = null;

// Implement IDecrypters when encrypted fields need to be read.
IDecrypters? decrypters = null;

// Execute a reader query asynchronously.
IEnumerable<Customer> rows =
    await CommandsAsync.ExecuteReaderAsync<Customer>(query, transaction, connection, decrypters);

// Execute a data-changing command asynchronously.
int affected =
    await CommandsAsync.ExecuteNonQueryAsync(query, transaction, connection);

// Execute a scalar command asynchronously.
object? result =
    await CommandsAsync.ExecuteScalarAsync(query, transaction, connection);
```

[ExampleEncryptor (AesGcm-based) and IDecrypters](#exampleencryptor-aesgcm-based-and-idecrypters)

[Table of Contents](#table-of-contents)

<a id="non-async-executenonquery--executescalar--executereadert"></a>

### Non-Async: `ExecuteNonQuery` / `ExecuteScalar` / `ExecuteReader<T>`

```csharp
Customer[] toDelete = [new Customer { Id = 7 }];
SqlQuery query = customerGenerator.DeleteById(toDelete);

NpgsqlConnection connection = new("Host=localhost;Database=AppDb;Username=app;Password=password");
NpgsqlTransaction? transaction = null;

IDecrypters? decrypters = null;

int affected = Commands.ExecuteNonQuery(query, transaction, connection);
object? scalar = Commands.ExecuteScalar(query, transaction, connection);
IEnumerable<Customer> customers =
    Commands.ExecuteReader<Customer>(query, transaction, connection, decrypters);
```

[Table of Contents](#table-of-contents)

---

<a id="simple-ado.net-example-with-sqlquery"></a>

## Simple ADO.NET Example With `SqlQuery`

If you prefer raw ADO.NET, you can still use `SqlQuery` for the SQL text, command type, and parameters. The generated PostgreSQL query text uses positional parameters such as `$1`, `$2`, and `$3`.

```csharp
using System;
using System.Collections.Generic;
using System.Data;
using Carrigan.SqlTools.PostgreSql;
using Carrigan.SqlTools.SqlGenerators;
using Npgsql;

// 1) Generate the query.
SqlQuery query = customerGenerator.SelectById(new Customer { Id = 42 });

// 2) Use Npgsql directly.
using NpgsqlConnection connection = new("Host=localhost;Database=AppDb;Username=app;Password=password");
connection.Open();

using NpgsqlCommand command = connection.CreateCommand();
command.CommandText = query.QueryText;
command.CommandType = query.CommandType;

// Add parameters in generated order.
// Positional placeholders such as $1 and $2 bind by parameter order.
foreach (KeyValuePair<string, object?> parameter in query.Parameters)
{
    command.Parameters.Add(new NpgsqlParameter { Value = parameter.Value ?? DBNull.Value });
}

using NpgsqlDataReader reader = command.ExecuteReader();
// ... map rows, or use the Carrigan.SqlTools client helpers ...
```

[Table of Contents](#table-of-contents)

---

## Data Type Mappings

### Default PostgreSQL Types

| C# CLR Type      | Default PostgreSQL Type |
|------------------|-------------------------|
| Guid             | UUID                    |
| string           | TEXT                    |
| char             | CHAR(1)                 |
| byte[]           | BYTEA                   |
| bool             | BOOLEAN                 |
| byte             | SMALLINT                |
| sbyte            | SMALLINT                |
| short            | SMALLINT                |
| ushort           | INTEGER                 |
| int              | INTEGER                 |
| uint             | BIGINT                  |
| long             | BIGINT                  |
| ulong            | NUMERIC(20, 0)          |
| float            | REAL                    |
| double           | DOUBLE PRECISION        |
| decimal          | NUMERIC                 |
| DateTime         | TIMESTAMP WITHOUT TIME ZONE |
| DateOnly         | DATE                    |
| TimeOnly         | TIME WITHOUT TIME ZONE  |
| TimeSpan         | INTERVAL                |
| DateTimeOffset   | TIMESTAMP WITH TIME ZONE |
| XDocument        | XML                     |
| XmlDocument      | XML                     |
| object           | TEXT fallback for type-based mapping; UNKNOWN for null value-based mapping |

[Table of Contents](#table-of-contents)

### Supported CLR Types

The PostgreSQL dialect supports nullable forms and array forms for the supported scalar types where applicable. `byte[]` is treated as PostgreSQL `BYTEA`, not as an array of small integers. `byte[][]` is supported as an array of `BYTEA` values.

Supported categories include:

- UUID values: `Guid`, `Guid?`, and arrays.
- Text values: `string`, `char`, `char?`, and arrays.
- Binary values: `byte[]` and `byte[][]`.
- Boolean values: `bool`, `bool?`, and arrays.
- Integer values: signed and unsigned integer types, nullable forms, and arrays.
- Numeric values: `float`, `double`, `decimal`, nullable forms, and arrays.
- Date and time values: `DateTime`, `DateOnly`, `TimeOnly`, `TimeSpan`, `DateTimeOffset`, nullable forms, and arrays.
- XML values: `XDocument`, `XmlDocument`, and arrays.
- Fallback values: `object` and `object[]`.

[Table of Contents](#table-of-contents)

### PostgreSQL Type Override Attributes

PostgreSQL override attributes derive from `SqlTypeAttribute` and can be applied to model properties to override the generated PostgreSQL type metadata.

| Attribute | PostgreSQL Type | Constructor Options | Intended CLR Types |
|-----------|-----------------|---------------------|--------------------|
| `PostgreSqlCharAttribute` | `CHAR` or `VARCHAR` | `StorageTypeEnum.Fixed` or `StorageTypeEnum.Var`; optional length | `char`, `string`; XML types are allowed but produce analyzer warnings |
| `PostgreSqlDateAttribute` | `DATE` | none | `DateOnly`; `DateTime` is allowed but produces an analyzer warning |
| `PostgreSqlTimeAttribute` | `TIME WITHOUT TIME ZONE` | optional fractional seconds precision `0` through `6` | `TimeOnly`; `DateTime` is allowed but produces an analyzer warning |
| `PostgreSqlTimestampAttribute` | `TIMESTAMP WITHOUT TIME ZONE` | optional fractional seconds precision `0` through `6` | `DateTime`; `DateOnly` and `TimeOnly` are allowed but produce analyzer warnings |
| `PostgreSqlTimestampUtcAttribute` | `TIMESTAMP WITH TIME ZONE` | optional fractional seconds precision `0` through `6` | `DateTimeOffset` |
| `PostgreSqlFloatAttribute` | `FLOAT` | optional precision `1` through `53` | `double`; `float` and `decimal` are allowed but may produce precision warnings |
| `PostgreSqlNumericAttribute` | `NUMERIC` | optional precision `1` through `255`; optional precision and scale | `decimal`; `float` and `double` are allowed but may produce precision warnings |
| `PostgreSqlMoneyAttribute` | `MONEY` | none | `decimal`, `double`, and `float`; produces a discouraged-type analyzer warning |

[Table of Contents](#table-of-contents)

### Analyzer Warnings

The PostgreSQL analyzer reports:

- An error when a PostgreSQL SQL type attribute is applied to an incompatible CLR property type.
- A precision warning when a floating-point CLR type is mapped to exact numeric SQL or when an exact CLR type is mapped to floating-point SQL.
- A semantic warning when a date-only or time-only CLR property is mapped to a full timestamp type, or when XML values are forced through character mappings.
- A discouraged-type warning when `PostgreSqlMoneyAttribute` is used. PostgreSQL supports `MONEY`, but `NUMERIC` is usually safer for new schemas.

No PostgreSQL-specific type override attribute is currently classified as obsolete by the analyzer.

[Table of Contents](#table-of-contents)

### Arrays

The PostgreSQL dialect supports PostgreSQL array declarations by rendering `[]` after the PostgreSQL type name.

Examples:

```text
INTEGER[] NOT NULL
NUMERIC(18, 2)[] NOT NULL
TIMESTAMP(6) WITH TIME ZONE[] NOT NULL
```

Array type detection is based on CLR array property types and supported SQL type attributes. `byte[]` is special-cased as `BYTEA`; it is not treated as an array declaration. `byte[][]` represents an array of `BYTEA` values.

[Table of Contents](#table-of-contents)

---

<a id="exampleencryptor-aesgcm-based-and-idecrypters"></a>

## ExampleEncryptor (AesGcm-based) and `IDecrypters`

Below is an example encryptor that uses an AesGcm-style API. Replace key handling with your own secure storage.

```csharp
//THIS IS JUST A SIMPLE EXAMPLE OF A ENCRYPTION CLASS
//I AM NOT A CRYPTOGRAPHIC EXPERT, DO NOT USE THIS EXAMPLE IN A REAL SYSTEM.

using System;
using System.Security.Cryptography;
using System.Text;
using Carrigan.Core.Interfaces;

public sealed class ExampleNonceGenerator : INonceGenerator
{
    public byte[] GenerateNonce()
    {
        byte[] nonce = new byte[12]; // 96-bit nonce
        RandomNumberGenerator.Fill(nonce);
        return nonce;
    }
}

public sealed class ExampleEncryptor : IEncryption
{
    private readonly byte[] _key;
    private const int TagSize = 16;

    public int? Version => 1;

    public byte[] KeyBytes => _key;

    public ExampleEncryptor(byte[] key)
    {
        if (key == null || key.Length != 32)
            throw new ArgumentException("Key must be 32 bytes.");

        _key = key;
    }

    public string? Encrypt(string? plainText)
    {
        if (plainText == null)
            return null;

        byte[] pt = Encoding.UTF8.GetBytes(plainText);
        byte[] ct = new byte[pt.Length];
        byte[] nonce = new byte[12];
        byte[] tag = new byte[TagSize];
        RandomNumberGenerator.Fill(nonce);

        using (AesGcm gcm = new AesGcm(_key, TagSize))
        {
            gcm.Encrypt(nonce, pt, ct, tag);
        }

        byte[] combined = new byte[nonce.Length + ct.Length + tag.Length];
        Buffer.BlockCopy(nonce, 0, combined, 0, nonce.Length);
        Buffer.BlockCopy(ct, 0, combined, nonce.Length, ct.Length);
        Buffer.BlockCopy(tag, 0, combined, nonce.Length + ct.Length, tag.Length);

        return Convert.ToBase64String(combined);
    }

    public string? Decrypt(string? cipherText)
    {
        if (cipherText == null)
            return null;

        byte[] combined = Convert.FromBase64String(cipherText);
        byte[] nonce = new byte[12];
        byte[] tag = new byte[TagSize];

        int cipherLen = combined.Length - nonce.Length - tag.Length;
        byte[] ct = new byte[cipherLen];

        Buffer.BlockCopy(combined, 0, nonce, 0, nonce.Length);
        Buffer.BlockCopy(combined, nonce.Length, ct, 0, ct.Length);
        Buffer.BlockCopy(combined, nonce.Length + ct.Length, tag, 0, tag.Length);

        byte[] pt = new byte[ct.Length];

        using (AesGcm gcm = new AesGcm(_key, TagSize))
        {
            gcm.Decrypt(nonce, ct, tag, pt);
        }

        return Encoding.UTF8.GetString(pt);
    }
}

// Minimal IDecrypters implementation mapping key versions to encryptors/decryptors

//THIS IS JUST A SIMPLE EXAMPLE OF A ENCRYPTION CLASS
//I AM NOT A CRYPTOGRAPHIC EXPERT, DO NOT USE THIS EXAMPLE IN A REAL SYSTEM.

public sealed class MyDecrypters : IDecrypters
{
    private readonly System.Collections.Generic.Dictionary<int, IEncryption> _map =
        new System.Collections.Generic.Dictionary<int, IEncryption>()
        {
            { 1, new ExampleEncryptor(RandomNumberGenerator.GetBytes(32)) }
        };

    public System.Collections.Generic.IEnumerable<int> Keys => _map.Keys;

    public IEncryption? Decryptor(int version)
    {
        return _map.TryGetValue(version, out IEncryption? encryptor) ? encryptor : null;
    }
}
```

Plug `MyDecrypters` into `ExecuteReaderAsync<T>` or `ExecuteReader<T>` to transparently decrypt properties annotated in your models.

[Table of Contents](#table-of-contents)

---

## License

Carrigan.SqlTools.Generators.PostgreSql
Copyright © 2025-2026 Carrigan Software Solutions LLC

Licensed under the Apache License, Version 2.0: http://www.apache.org/licenses/LICENSE-2.0

[Table of Contents](#table-of-contents)

---

## Disclaimers

This project is not affiliated with or endorsed by the PostgreSQL project.

This library may generate or execute SQL. Review generated SQL before use in production systems. Always test against non-production databases first.

Use caution with schema, migration, and data-modifying operations. The authors are not responsible for data loss, downtime, or unintended database changes.
