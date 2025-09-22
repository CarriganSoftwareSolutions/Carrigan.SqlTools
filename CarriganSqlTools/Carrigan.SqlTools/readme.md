# Carrigan.SqlTools  
<!--Ignore Spelling: executenonqueryasync executescalarasync executereaderasynct executenonquery executescalar executereadert adonet sqlquery exampleencryptor aesgcm idecryptors dotnet csharp nameof foreach readonly const gcm decryptor decryptors encryptor encryptors-->
Carrigan.SqlTools is a .NET library that simplifies SQL generation for **Microsoft SQL Server**, while still giving you control when you need it.  

It automatically generates `SELECT`, `INSERT`, `UPDATE`, and `DELETE` statements using reflection. **Carrigan.Core** provides interfaces and property attributes to integrate your custom field-level encryption, and Carrigan.SqlTools adds a safe, object-oriented API for building more advanced queries.  

A companion library, **Carrigan.SqlTools.SqlServer**, extends functionality by wrapping ADO.NET to execute generated queries, map rows to objects, and handle decryption of encrypted fields.

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
  - [Delete with Join and Where](#delete-with-join-and-where)
  - [Select Count With Where](#select-count-with-where)
  - [Update with Joins and Where](#update-with-joins-and-where)
- [Attribute Examples](#attribute-examples)
  - [Table, Column and Key](#table-and-column)
  - [Identifier and Primary Key](#identifier-and-primary-key)
  - [Procedure and Parameter](#procedure-and-parameter)
- [Running Queries (Async & Non-Async)](#running-queries-async--non-async)
  - [Async: ExecuteNonQueryAsync / ExecuteScalarAsync / ExecuteReaderAsync\<T>](#async-executenonqueryasync--executescalarasync--executereaderasynct)
  - [Non-Async: ExecuteNonQuery / ExecuteScalar / ExecuteReader\<T>](#non-async-executenonquery--executescalar--executereadert)
- [Simple ADO.NET Example With SqlQuery](#simple-adonet-example-with-sqlquery)
- [ExampleEncryptor (AesGcm-based) and IDecryptors](#exampleencryptor-aesgcm-based-and-idecryptors)
- [License](#license)

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

[Table of Contents](#table-of-contents)

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

[Table of Contents](#table-of-contents)

---

## Getting Started Examples

We use `SqlGenerator<T>` to produce a **`SqlQuery`** (with `QueryText`, `CommandType`, and `Parameters`). 

You can use the `[Table]` attribute from `System.ComponentModel.DataAnnotations.Schema` to override the table name, otherwise the table name is assumed to be the same as the class. You can also specify the schema name or not.

All examples use `using` statements to keep code clean, and initialize generators **with an encryptor** (required).

```csharp
using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.OffsetNexts;
using Carrigan.SqlTools.OrderByItems;
using Carrigan.SqlTools.Predicates;
using Carrigan.SqlTools.Sets;
using Carrigan.SqlTools.SqlGenerators;
using System;
using System.Collections.Generic;
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


// Generators
SqlGenerator<Customer> customerGenerator = new ();
SqlGenerator<Order>    orderGenerator    = new ();
```

[Table of Contents](#table-of-contents)


### Select All Rows

```csharp
SqlQuery query = customerGenerator.SelectAll();
// query.QueryText → SELECT [Customer].* FROM [Customer]
```

[Table of Contents](#table-of-contents)


### Select by Id
Key attribute required, and composite keys are supported by specifying multiple Keys.
```csharp
Customer entity = new() { Id = 42 };
SqlQuery query = customerGenerator.SelectById(entity);
// SELECT [Customer].* FROM [Customer] WHERE ([Customer].[Id] = @Parameter_Id)
```

[Table of Contents](#table-of-contents)


### Insert

```csharp
Customer entity = new() 
{   
    Id = 42, Name = "Hank", 
    Email = "Hank@example.com", 
    Phone = "+1(555)555-5555" 
};
SqlQuery query = customerGenerator.Insert(entity);

// INSERT INTO [Customer] ([Id], [Name], [Email], [Phone]) 
// VALUES (@Id, @Name, @Email, @Phone);
```

[Table of Contents](#table-of-contents)


### Insert with Auto Id
Key attribute required, and Id columns must have a default value.
```csharp
Customer entity = new() 
{ 
    Name = "Hank", 
    Email = "Hank@example.com",
    Phone= "+1(555)555-5555" 
};
SqlQuery query = customerGenerator.InsertAutoId(entity);

// DECLARE @OutputTable TABLE (InsertedId UNIQUEIDENTIFIER);
// INSERT INTO [Customer] ([Name], [Email], [Phone]) 
// OUTPUT INSERTED.Id INTO @OutputTable 
// VALUES (@Name, @Email, @Phone);
// SELECT InsertedId FROM @OutputTable;
```

[Table of Contents](#table-of-contents)


### Update by Id
Key attribute required, and composite keys are supported by specifying multiple Keys.
```csharp
Customer entity = new() 
{ 
    Id = 42, 
    Name = "Hank Hill", 
    Email = "Hank.Hill@example.com",
    Phone = "+1(555)555-5555"
};
SqlQuery query = customerGenerator.UpdateById(entity);

// UPDATE [Customer] 
// SET [Name] = @Name, [Email] = @Email, [Phone] = @Phone
// WHERE [Id] = @Id;
```

[Table of Contents](#table-of-contents)


### Update by Id (selected columns)
Key attribute required, and composite keys are supported by specifying multiple Keys. 

`SetColumns<T>` validates the names of the properties, and throws an error if the property isn't valid
```csharp
SetColumns<Customer> columns = new(nameof(Customer.Email));
Customer entity = new() { Id = 42, Name = "Hank", Email = "Hank@example.gov" };
SqlQuery query = customerGenerator.UpdateById(entity, columns);
// UPDATE [Customer] SET [Email] = @Email WHERE [Id] = @Id;
```

[Table of Contents](#table-of-contents)


### Delete
Key attribute required, and composite keys are supported by specifying multiple Keys.
```csharp
Customer entity = new() { Id = 42 };
SqlQuery query = customerGenerator.Delete(entity);
// DELETE FROM [Customer] WHERE [Id] = @Id;
```

[Table of Contents](#table-of-contents)


### Delete by Id (multiple keys)
Key attribute required, and composite keys are supported by specifying multiple Keys.
```csharp
Customer[] entities = new Customer[] { new Customer { Id = 1 }, new Customer { Id = 2 } };
SqlQuery query = customerGenerator.DeleteById(entities);
// ... WHERE [Id] = @Id OR [Id] = @Id_1
```

[Table of Contents](#table-of-contents)

---

## More Complex Examples

### Select with Joins and Order By
`ColumnEqualsColumn<LeftT, RightT>`, Order validates the names of the properties, and throws an error if the property isn't valid

`OrderByItem<Order>` validates the names of the properties, and throws an error if the property isn't valid
```csharp
ColumnEqualsColumn<Customer, Order> columnEqualsColumn = new(nameof(Customer.Id), nameof(Order.CustomerId));
InnerJoin<Customer, Order> join = new(columnEqualsColumn);

OrderByItem<Order> orderByOrderDate = new(nameof(Order.OrderDate));

SqlQuery query = customerGenerator.Select(join, null, orderByOrderDate, null);

// SELECT [Order].* FROM [Order] 
// INNER JOIN [Order] ON 
// ([Customer].[Id] = [Order].[CustomerId]) 
// ORDER BY [Order].[OrderDate] ASC
```

[Table of Contents](#table-of-contents)


### Select with Two Part Order By
`ColumnEqualsColumn<LeftT, RightT>`, Order validates the names of the properties, and throws an error if the property isn't valid

`OrderByItem<Order>` validates the names of the properties, and throws an error if the property isn't valid
```csharp
ColumnEqualsColumn<Customer, Order> columnEqualsColumn = new(nameof(Customer.Id), nameof(Order.CustomerId));

InnerJoin<Customer, Order> join = new(columnEqualsColumn);

OrderByItem<Order> orderByOrderDate = new(nameof(Order.OrderDate));
OrderByItem<Customer> orderByCustomerId = new(nameof(Customer.Id), SortDirectionEnum.Descending);
OrderBy orderBy = new(orderByCustomerId, orderByOrderDate);

SqlQuery query = customerGenerator.Select(join, null, orderBy, null);

// SELECT [Order].* FROM [Order] 
// INNER JOIN [Order] ON 
// ([Customer].[Id] = [Order].[CustomerId]) 
// ORDER BY [Customer].[Id] DESC, [Order].[OrderDate] ASC
```

[Table of Contents](#table-of-contents)


### Delete with Join and Where
`ColumnEqualsColumn<LeftT, RightT>` validates the names of the properties, and throws an error if the property isn't valid

`ColumnValues<T>` validates the names of the properties, and throws an error if the property isn't valid
```csharp
ColumnEqualsColumn<Customer, Order> columnEqualsColumn = new(nameof(Customer.Id), nameof(Order.CustomerId));

InnerJoin<Order, Customer> join = new(columnEqualsColumn);

ColumnValues<Customer> customerEmail = new(nameof(Customer.Email), "spam@example.com");

SqlQuery query = orderGenerator.Delete(join, customerEmail);

// DELETE FROM [Order] 
// INNER JOIN [Customer] ON 
// ([Customer].[Id] = [Order].[CustomerId]) 
// WHERE ([Customer].[Email] = @Parameter_Email)
```

[Table of Contents](#table-of-contents)


### Select Count With Where

`Columns<T>` validates the names of the properties, and throws an error if the property isn't valid

```csharp
Columns<Order> totalCol = new (nameof(Order.Total));
Parameters minTotal = new ("Total", 500m);
GreaterThan greaterThan = new (totalCol, minTotal);

SqlQuery query = orderGenerator.SelectCount(null, greaterThan);

// SELECT COUNT(*) FROM [Order] WHERE ([Order].[Total] > @Parameter_Total)
```

[Table of Contents](#table-of-contents)


### Update with Joins and Where
`SetColumns<T>` validates the names of the properties, and throws an error if the property isn't valid

`ColumnEqualsColumn<LeftT, RightT>` validates the names of the properties, and throws an error if the property isn't valid

`ColumnValues<T>` validates the names of the properties, and throws an error if the property isn't valid

```csharp
Order entity = new () { Id = 10, Total = 123.45m };

SetColumns<Order> setColumns = new(nameof(Order.Total));

ColumnEqualsColumn<Order, Customer> columnEqualsColumn = new(nameof(Order.CustomerId), nameof(Customer.Id));

InnerJoin<Order, Customer> joinOnCustomerId = new(columnEqualsColumn);

ColumnValues<Customer> customerEmailEquals = new(nameof(Customer.Email), "spam@example.com");

SqlQuery query = orderGenerator.Update(entity, setColumns, joinOnCustomerId, customerEmailEquals);

// UPDATE [Order] SET [Order].[Total] = @ParameterSet_Total 
// FROM [Order] 
// INNER JOIN [Customer] ON 
// ([Order].[CustomerId] = [Customer].[Id]) 
// WHERE ([Customer].[Email] = @Parameter_Email)
```

[Table of Contents](#table-of-contents)


---

## Attribute Examples
### Table, Column and Key

```csharp
[Table("Phone", Schema = "schema")]
public class PhoneModel
{
    [Key]
    public int Id { get; set; }
    public int CustomerId { get; set; }
    [Column("Phone")]
    public string? PhoneNumber { get; set; }
}

SqlGenerator<PhoneModel> phoneGenerator = new();

PhoneModel phone = new()
{
    Id = 2718,
    CustomerId = 3141,
    PhoneNumber = "+15555555555"
};
SqlQuery query = phoneGenerator.UpdateById(phone);

// UPDATE [schema].[Phone] 
// SET [CustomerId] = @CustomerId, [Phone] = @Phone 
// WHERE [Id] = @Id;
```

[Table of Contents](#table-of-contents)

### Identifier and Primary Key

```csharp
[Identifier("Email", "schema")]
public class EmailModel
{
    [PrimaryKey]
    public int Id { get; set; }
    public int CustomerId { get; set; }
    [Identifier("Email")]
    public string? EmailAddress { get; set; }
}

SqlGenerator<EmailModel> emailGenerator = new();

EmailModel email = new()
{
    Id = 10,
    CustomerId = 313,
    EmailAddress = "Exterminate@Skaro.gov"
};
SqlQuery query = emailGenerator.UpdateById(email);

// UPDATE [schema].[Email] 
// SET [CustomerId] = @CustomerId, [Email] = @Email 
// WHERE [Id] = @Id;
```

[Table of Contents](#table-of-contents)

### Procedure and Parameter

```csharp
[Identifier("UpdateThing", "schema")]
public class ProcedureExec
{
    [Parameter ("SomeValue")]
    public string? ValueColumn { get; set; }
}

SqlGenerator<ProcedureExec> procedureExecGenerator = new();

ProcedureExec procedureExec = new()
{
    ValueColumn = "DangItBobby"
};
SqlQuery query = procedureExlGenerator.Procedure(procedureExec);

// [schema].[UpdateThing]
```

[Table of Contents](#table-of-contents)

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

SqlConnection connection = new SqlConnection(<your connection string>);
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
[Example Encryptor (AesGcm-based) and IDecryptors](#exampleencryptor-aesgcm-based-and-idecryptors)

[Table of Contents](#table-of-contents)


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

[Table of Contents](#table-of-contents)


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

[Table of Contents](#table-of-contents)


---

## ExampleEncryptor (AesGcm-based) and `IDecryptors`

Below is an example encryptor that uses an AesGcm-style API. Replace key handling with your own secure storage.

```csharp
//THIS IS JUST A SIMPLE EXAMPLE OF A ENCRYPTION CLASS
//I AM NOT A CRYPTOGRAPHIC EXPERT, DO NOT USE THIS EXAMPLE IN A REAL SYSTEM.

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

    
    public int? Version => 1;

    public byte[] KeyBytes => _key;

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

// Minimal IDecryptors imply mapping versions → encryptors

//THIS IS JUST A SIMPLE EXAMPLE OF A ENCRYPTION CLASS
//I AM NOT A CRYPTOGRAPHIC EXPERT, DO NOT USE THIS EXAMPLE IN A REAL SYSTEM.


public sealed class MyDecryptors : IDecryptors
{
    private readonly System.Collections.Generic.Dictionary<int, IEncryption> _map =
        new System.Collections.Generic.Dictionary<int, IEncryption>()
        {
            { 1, new ExampleEncryptor(RandomNumberGenerator.GetBytes(32)) }
        };

    public System.Collections.Generic.IEnumerable<int> Keys => _map.Keys;

    public IEncryption? Decryptor(int version)
    {
        return _map.TryGetValue(version, out IEncryption enc) ? enc : null;
    }
}


//THIS IS JUST A SIMPLE EXAMPLE OF A ENCRYPTION CLASS
//I AM NOT A CRYPTOGRAPHIC EXPERT, DO NOT USE THIS EXAMPLE IN A REAL SYSTEM.
```

> Plug `MyDecryptors` into `ExecuteReaderAsync<T>` / `ExecuteReader<T>` to transparently decrypt fields annotated in your models.

[Table of Contents](#table-of-contents)


---

## License

Carrigan.SqlTools  
Copyright © 2025 Carrigan Software Solutions LLC

Licensed under the Apache License, Version 2.0: http://www.apache.org/licenses/LICENSE-2.0

[Table of Contents](#table-of-contents)

