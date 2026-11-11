# Carrigan.SqlTools  

Carrigan.SqlTools is a .NET library that simplifies SQL generation for **Microsoft SQL Server**, while still giving you control when you need it.

It automatically generates `SELECT`, `INSERT`, `UPDATE`, and `DELETE` statements using reflection, **Carrigan.Core** provides an interfaces and property attributes to integrate your custom field level encryption, and provides a safe, object-oriented API for building more advanced queries.  


A companion library, **Carrigan.SqlTools.SqlServer**, extends functionality by wrapping ADO.NET to execute generated queries, invoke object mapping, and handle decryption of encrypted data.  

---

## Framework Compatibility and Support

These libraries provide binaries for .NET 9, .NET 10, and .NET 11. **Official support is limited to .NET versions that Microsoft currently supports.**

Microsoft ended support for .NET 9 on November 10, 2026. We continue to provide `net9.0` binaries for compatibility with existing applications, but binaries targeting unsupported .NET versions are provided **as-is and without official support**. Framework-specific testing, bug fixes, security updates, and technical support are not guaranteed for those targets.

Support for obsolete targets may be discontinued in a future release. We recommend using a Microsoft-supported .NET version for new projects and production applications.

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
