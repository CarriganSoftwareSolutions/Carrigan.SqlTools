# Carrigan.SqlTools  

Carrigan.SqlTools is a .NET library that simplifies SQL generation for **Microsoft SQL Server**, while still giving you control when you need it.  

It automatically generates `SELECT`, `INSERT`, `UPDATE`, and `DELETE` statements using reflection. **Carrigan.Core** provides interfaces and property attributes to integrate your custom field-level encryption, and Carrigan.SqlTools adds a safe, object-oriented API for building more advanced queries.  

A companion library, **Carrigan.SqlTools.SqlServer**, extends functionality by wrapping ADO.NET to execute generated queries, map rows to objects, and handle decryption of encrypted fields.

---

## Features  

- **Automatic CRUD generation**  
  Build `SELECT`, `INSERT`, `UPDATE`, and `DELETE` queries with reflection.

- **Carrigan.Core integration**  
  Interfaces and property-level attributes enable field-level encryption and decryption.

- **Manual query builder**  
  Safely construct advanced SQL with `JOIN`, predicates (e.g., `Equal`, `GreaterThan`, `IsNull`, `Like`, `And`, `Or`, `Xor`, `Not`), `ORDER BY`, and pagination (`OFFSET/FETCH NEXT`).

- **Dictionary → object mapping**  
  Use the invocation system to populate typed models from database rows.

- **Focused on SQL Server**  
  SQL output targets Microsoft SQL Server.

- **Execution helpers**  
  Async commands (`CommandsAsync`) and non-async commands (`Commands`) for running queries and reading results.

---

## Installation  

Carrigan.SqlTools is available as a NuGet package:

```powershell
dotnet add package Carrigan.SqlTools
```

For SQL Server execution helpers:

```powershell
dotnet add package Carrigan.SqlTools.SqlServer
```

---

## Table of Contents for Examples

- [Getting Started Examples](#getting-started-examples)
  - [Select All Rows](#select-all-rows)
  - [Select by Id](#select-by-id)
  - [Insert](#insert)
  - [Insert with Auto Id](#insert-with-auto-id)
  - [Update by Id](#update-by-id)
  - [Update by Id (selected columns)](#update-by-id-selected-columns)
  - [Delete](#delete)
  - [Delete by Id (multiple keys)](#delete-by-id-multiple-keys)
  - [Select with Joins](#select-with-joins)
  - [Delete with Join and Where](#delete-with-join-and-where)
  - [Select Count (with predicates)](#select-count-with-predicates)
  - [Update with Joins and Predicates](#update-with-joins-and-predicates)
- [Running Queries (Async & Non-Async)](#running-queries-async--non-async)
  - [Async: ExecuteNonQueryAsync / ExecuteScalarAsync / ExecuteReaderAsync\<T>](#async-executenonqueryasync--executescalarasync--executereaderasynct)
  - [Non-Async: ExecuteNonQuery / ExecuteScalar / ExecuteReader\<T>](#non-async-executenonquery--executescalar--executereadert)
- [Simple ADO.NET Example With SqlQuery](#simple-adonet-example-with-sqlquery)
- [ExampleEncryptor (AesGcm-based) and IDecryptors](#exampleencryptor-aesgcm-based-and-idecryptors)
- [License](#license)

---

## Getting Started Examples

We use `SqlGenerator<T>` to produce a **`SqlQuery`** (with `QueryText`, `CommandType`, and `Parameters`). 

You can use the `[Table]` attribute from `System.ComponentModel.DataAnnotations.Schema` to override the table name, otherwise the table name is assumed to be the same as the class. You can also specify the schema name or not.

All examples use `using` statements to keep code clean, and initialize generators **with an encryptor** (required).

```csharp
using System;
using System.Collections.Generic;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Predicates;
using Carrigan.SqlTools.Joins;
using Carrigan.SqlTools.Ordering;
using Carrigan.SqlTools.Paging;
using Carrigan.SqlTools.Sets;
using System.ComponentModel.DataAnnotations;

// Example data models
public class Customer
{
    [Key] //Required attribute for certain SQL Generations
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
}

public class Order
{
    [Key] //Required attribute for certain SQL Generations
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal Total { get; set; }
}

// Minimal example encryptor (from the ExampleEncryptor section below or your own implementation)
ExampleEncryptor exampleEncryptor = new ExampleEncryptor(new byte[32] {
    1,2,3,4,5,6,7,8,9,10, 11,12,13,14,15,16,
    17,18,19,20,21,22,23,24,25,26, 27,28,29,30,31,32
});

// Generators (encryptor is required)
SqlGenerator<Customer> customerGenerator = new (exampleEncryptor);
SqlGenerator<Order>    orderGenerator    = new (exampleEncryptor);
```

### Select All Rows

```csharp
SqlQuery query = customerGenerator.SelectAll();
// query.QueryText → SELECT [Customer].* FROM [Customer];
```

### Select by Id
Key attribute required, and composite keys are supported by specifying multiple Keys.
```csharp
Customer filter = new Customer { Id = 42 };
SqlQuery query = customerGenerator.SelectById(filter);
// ... WHERE [Customer].[Id] = @Id
```

### Insert

```csharp
Customer entity = new Customer { Id = 42,  Name = "Alice", Email = "alice@example.com" };
SqlQuery query = customerGenerator.Insert(entity);
// INSERT INTO [Customer] ([Name], [Email]) VALUES (@Name, @Email);
```

### Insert with Auto Id
Key attribute required, and Id columns must have a default value.
```csharp
Customer entity = new Customer { Name = "Alice", Email = "alice@example.com" };
SqlQuery query = customerGenerator.InsertAutoId(entity);
// INSERT INTO [Customer] ([Name], [Email]) VALUES (@Name, @Email);
```

### Update by Id
Key attribute required, and composite keys are supported by specifying multiple Keys.
```csharp
Customer entity = new Customer { Id = 42, Name = "Alice Smith", Email = "alice.smith@example.com" };
SqlQuery query = customerGenerator.UpdateById(entity);
// UPDATE [Customer] SET [Name] = @Name, [Email] = @Email WHERE [Id] = @Id;
```

### Update by Id (selected columns)
Key attribute required, and composite keys are supported by specifying multiple Keys.
```csharp
SetColumns<Customer> columns = new SetColumns<Customer>("Name", "Email");
Customer entity = new Customer { Id = 42, Name = "Alice Updated", Email = "alice.updated@example.com" };
SqlQuery query = customerGenerator.UpdateByIdColumns(entity, columns);
```

### Delete
Key attribute required, and composite keys are supported by specifying multiple Keys.
```csharp
Customer entity = new Customer { Id = 42 };
SqlQuery query = customerGenerator.Delete(entity);
// DELETE FROM [Customer] WHERE [Id] = @Id;
```

### Delete by Id (multiple keys)
Key attribute required, and composite keys are supported by specifying multiple Keys.
```csharp
Customer[] entities = new Customer[] { new Customer { Id = 1 }, new Customer { Id = 2 } };
SqlQuery query = customerGenerator.DeleteById(entities);
// ... WHERE [Id] = @Id OR [Id] = @Id_1
```

### Select with Joins

```csharp
// Build components explicitly (no lambdas)
InnerJoin<Customer, Order> join = new InnerJoin<Customer, Order>("Id", "CustomerId");
OrderBy orderBy = new OrderBy(new OrderByItem<Order>("OrderDate"));

SqlQuery query = orderGenerator.Select(join, null, orderBy, null);
```

### Delete with Join and Where

```csharp
InnerJoin<Customer, Order> join = new InnerJoin<Customer, Order>("Id", "CustomerId");
Columns<Customer> emailCol = new Columns<Customer>("Email");
Parameters emailParam = new Parameters("Email", "spam@example.com");
Equal emailEquals = new Equal(emailCol, emailParam);

SqlQuery query = orderGenerator.Delete(join, emailEquals);
```

### Select Count (with predicates)

```csharp
Columns<Order> totalCol = new Columns<Order>("Total");
Parameters minTotal = new Parameters("Total", 500m);
GreaterThan greaterThan = new GreaterThan(totalCol, minTotal);

SqlQuery query = orderGenerator.SelectCount(predicates: greaterThan);
```

### Update with Joins and Predicates

```csharp
SetColumns<Order> setColumns = new SetColumns<Order>("Total");
Order entity = new Order { Id = 10, Total = 123.45m };

InnerJoin<Customer, Order> join = new InnerJoin<Customer, Order>("Id", "CustomerId");
Columns<Customer> nameCol = new Columns<Customer>("Name");
Parameters emailParam = new Parameters("Email", "spam@example.com");
Equal nameEquals = new Equal(nameCol, emailParam);

SqlQuery query = orderGenerator.UpdateWithJoinsAndPredicates(entity, setColumns, join, nameEquals);
```

---

## Running Queries (Async & Non-Async)

The SQL Server helpers live in **Carrigan.SqlTools.SqlServer**.  
These examples use `using` statements to keep code tidy.

```csharp
using System.Collections.Generic;
using System.Data.Common;
using Carrigan.Core.Interfaces;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.SqlServer;
using Microsoft.Data.SqlClient;
```

### Async: `ExecuteNonQueryAsync` / `ExecuteScalarAsync` / `ExecuteReaderAsync<T>`

```csharp
// Build a query (example: SelectAll)
SqlQuery query = customerGenerator.SelectAll();

SqlConnection connection = new SqlConnection("Server=.;Database=AppDb;Integrated Security=true;");
DbTransaction transaction = null;

// Implement IDecryptors (see ExampleEncryptor section)
IDecryptors decryptors = new MyDecryptors();

// Execute (async)
IEnumerable<Customer> rows =
    await CommandsAsync.ExecuteReaderAsync<Customer>(query, transaction, connection, decryptors);

// Write/update (async)
int affected =
    await CommandsAsync.ExecuteNonQueryAsync(query, transaction, connection);

// Single scalar (async)
object result =
    await CommandsAsync.ExecuteScalarAsync(query, transaction, connection);
```

### Non-Async: `ExecuteNonQuery` / `ExecuteScalar` / `ExecuteReader<T>`

```csharp
Customer[] toDelete = new Customer[] { new Customer { Id = 7 } };
SqlQuery query = customerGenerator.DeleteById(toDelete);

SqlConnection connection = new SqlConnection("Server=.;Database=AppDb;Integrated Security=true;");
DbTransaction transaction = null;

// IDecryptors (same interface as async)
IDecryptors decryptors = new MyDecryptors();

// Execute (sync)
int affected = Commands.ExecuteNonQuery(query, transaction, connection);
object scalar = Commands.ExecuteScalar(query, transaction, connection);
IEnumerable<Customer> customers =
    Commands.ExecuteReader<Customer>(query, transaction, connection, decryptors);
```

---

## Simple ADO.NET Example With `SqlQuery`

If you prefer raw ADO.NET, you can still use `SqlQuery` for the SQL text, command type, and parameters:

```csharp
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Carrigan.SqlTools.SqlGenerators;
using Microsoft.Data.SqlClient;

// 1) Generate the query
SqlQuery query = customerGenerator.SelectById(new Customer { Id = 42 });

// 2) Use ADO.NET directly
SqlConnection connection = new SqlConnection("Server=.;Database=AppDb;Integrated Security=true;");
connection.Open();

DbCommand command = connection.CreateCommand();
command.CommandText = query.QueryText;   // ← from SqlQuery
command.CommandType = query.CommandType; // ← from SqlQuery

// Add parameters
foreach (KeyValuePair<string, object?> p in query.Parameters)
{
    command.Parameters.Add(new SqlParameter(p.Key, p.Value ?? DBNull.Value));
}

// Execute
DbDataReader reader = command.ExecuteReader();
// ... map rows (or use invoker) ...
reader.Close();
connection.Close();
```

---

## ExampleEncryptor (AesGcm-based) and `IDecryptors`

Below is an example encryptor that uses an AesGcm-style API. Replace key handling with your own secure storage.

```csharp
using System;
using System.Text;
using System.Security.Cryptography;
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

    public ExampleEncryptor(byte[] key)
    {
        if (key == null || key.Length != 32) throw new ArgumentException("Key must be 32 bytes.");
        _key = key;
    }

    public string? Encrypt(string? plainText)
    {
        if (plainText == null) return null;

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
        if (cipherText == null) return null;

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

// Minimal IDecryptors impl mapping versions → encryptors
public sealed class MyDecryptors : IDecryptors
{
    private readonly System.Collections.Generic.Dictionary<int, IEncryption> _map =
        new System.Collections.Generic.Dictionary<int, IEncryption>()
        {
            { 1, new ExampleEncryptor(new byte[32] { 1,2,3,4,5,6,7,8,9,10, 11,12,13,14,15,16, 17,18,19,20,21,22,23,24,25,26, 27,28,29,30,31,32 }) }
        };

    public System.Collections.Generic.IEnumerable<int> Keys => _map.Keys;

    public IEncryption? Decryptor(int version)
    {
        return _map.TryGetValue(version, out IEncryption enc) ? enc : null;
    }
}
```

> Plug `MyDecryptors` into `ExecuteReaderAsync<T>` / `ExecuteReader<T>` to transparently decrypt fields annotated in your models.

---

## License

Carrigan.SqlTools  
Copyright © 2025 Carrigan Software Solutions LLC

Licensed under the Apache License, Version 2.0: http://www.apache.org/licenses/LICENSE-2.0
