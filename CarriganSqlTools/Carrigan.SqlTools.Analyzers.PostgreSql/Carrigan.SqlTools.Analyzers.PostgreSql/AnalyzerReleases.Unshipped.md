Rule ID | Category | Severity | Notes
----- | ----- | ----- | -----
CARRIGANSQL0011 | Usage | Error | Enforces type safety with the use of the various attributes that inherit from SqlTypeAttribute
CARRIGANSQL0012 | Usage | Warning | Possible precision mismatch when a SQL type attribute is applied to a code type with differing numeric precision/scale.
CARRIGANSQL0013 | Usage | Warning | Possible semantic mismatch between the PostgreSQL SQL type attribute (e.g., date vs. time vs. timestamp) and the code type.
CARRIGANSQL0014 | Usage | Warning | Use of attributes associated with discouraged PostgreSQL SQL types (e.g., MONEY) where NUMERIC is usually preferred for new schemas.
CARRIGANSQL0015 | Usage | Warning | Use of attributes associated with obsolete PostgreSQL SQL types that should be avoided in new development.
