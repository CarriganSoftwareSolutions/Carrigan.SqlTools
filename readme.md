# Carrigan.SqlTools  

Carrigan.SqlTools is a .NET library that simplifies SQL generation for **Microsoft SQL Server**, while still giving you control when you need it.  

It automatically generates `SELECT`, `INSERT`, `UPDATE`, and `DELETE` statements using reflection, **Carrigan.Core** provides an interfaces and property attributes to integrate your custom field level encryption, and provides a safe, object-oriented API for building more advanced queries.  


A companion library, **Carrigan.SqlTools.SqlServer**, extends functionality by wrapping ADO.NET to execute generated queries, invoke object mapping, and handle decryption of encrypted data.  

---

## Features  

- **Automatic CRUD generation**  
  Build `SELECT`, `INSERT`, `UPDATE`, and `DELETE` queries with reflection.  

- **Carrigan.Core integration**  
  Provides interfaces and property-level attributes for implementing your encryption logic at a field level.  

- **Manual query builder**  
  Safely construct advanced SQL with `ORDER BY`, `JOIN`, `WHERE`, and pagination support.  

- **Dictionary to object mapping**  
  Use an invocation class to map `Dictionary<string, object?>` values directly to typed instances.  

- **Focused on SQL Server**  
  SQL output is tailored for Microsoft SQL Server.  

- **Companion library support**  
  `Carrigan.SqlTools.SqlServer` adds wrappers for ADO.NET execution and encrypted data handling.  

---

## Installation  

Carrigan.SqlTools is available as a NuGet package:  

```powershell
dotnet add package Carrigan.SqlTools
