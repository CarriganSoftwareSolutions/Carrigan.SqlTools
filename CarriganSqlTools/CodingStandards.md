# Carrigan.SqlTools Coding & Documentation Standards

## General Coding Standards

- Use **target-typed `new(...)`** without the constructor name when possible.
- Use **expression-bodied members** when possible.
- **Never use `var`** unless absolutely necessary.
- Use **file-scoped namespaces**.
- Never use **block (“flower boxing”) comments**.
- Use **variables with full, unambiguous names**.
  - **Exception:** Data types may be used as variable names when referencing the datatype itself  
    *Example: `GetInt`*.
- Allow abbreviations **only when the variable name is identical to the datatype name**.  
    *Valid Example: EncodingEnum encodingEnum;*
- **Never use `continue`** statements.
- Use **collection expressions** when possible.

---

## Assumed Global Usings

The following namespaces are assumed to be globally imported in all projects:

```csharp
global using global::System;
global using global::System.Collections.Generic;
global using global::System.IO;
global using global::System.Linq;
global using global::System.Net.Http;
global using global::System.Threading;
global using global::System.Threading.Tasks;
```

---

# Unit Test Standards

- Assume **internals are exposed** to the unit testing project  
  (e.g., using `InternalsVisibleTo`).  
  Unit tests should take advantage of this and **test the code as thoroughly as possible**.

- For xUnit tests, always prefer **`[InlineData]`** over `MemberData`.

- Test method names must be:
  - Clear  
  - Descriptive  
  - Written in standard xUnit naming conventions  

- When testing constructors expected to throw exceptions, always use:

```csharp
Assert.Throws<SomeException>(() => new SomeType(...));
```

- Unit tests assume the additional global using:

```csharp
global using global::Xunit;
```

---

## Unit Test Method Naming Rules

When naming unit test methods using `_` separators:

- Keep the number of sections **as small as needed** to differentiate between similarly named tests.
- Use the pattern:

```
BasicName
BasicName_UniqueAspect
BasicName_UniqueAspect_Exception
```

- Only include a **result section** (e.g., `Exception`, `Error`) when the test expects an exception or error.
- **Normal tests** should contain **no more than two sections** (one `_`).
- **Exception tests** should contain **no more than three sections** (two `_`).

### Examples

```
Constructor
Constructor_NoArguments
Constructor_WithArguments
Constructor_WithArguments_Exception
```

---

# Documentation Standards for Carrigan.SqlTools

Carrigan.SqlTools is an **SQL modeling and SQL-generation library**.

When documenting code that generates or references columns and tables from a data model, always use terminology that clearly reflects how the library models SQL concepts:

- **Classes** represent **tables** or **stored procedures**.
- **Properties** represent **columns** (for tables) or **parameters** (for procedures).

Documentation must communicate these roles clearly so the relationship between the C# model and the SQL schema is unambiguous.

For example:

When an attribute is applied to a property, **the attribute does not merely define “a column name.”**  
Instead, documentation must explicitly state that:

> The attribute defines metadata for the *property*, and that property represents an SQL column in the data model.  
> The attribute therefore controls or overrides the metadata used when generating the SQL column definition.

This distinction is essential for ensuring that users of Carrigan.SqlTools understand how:

- SQL metadata flows from C# models  
- Reflection is used to interpret table/column mappings  
- Attributes shape the resulting SQL definitions  
- Validation and SQL-generation logic interpret the model  

All documentation should help reinforce this **C# ? SQL mapping architecture**, making the design intent clear throughout the library.
