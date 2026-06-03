Rule ID | Category | Severity | Notes
----- | ----- | ----- | -----
CARRIGANSQL0001 | Usage | Error | Enforces type safety with the use of the various attributes that inherit from SqlTypeAttribute
CARRIGANSQL0002 | Usage | Warning | Possible precision mismatch when a SQL type attribute is applied to a code type with differing numeric precision/scale.
CARRIGANSQL0003 | Usage | Warning | Possible semantic mismatch between the SQL type attribute (e.g., date vs. time vs. datetime) and the code type.
CARRIGANSQL0004 | Usage | Warning | Use of attributes associated with legacy SQL types (e.g., DATETIME, MONEY) that have reduced range or scale compared to modern equivalents.
CARRIGANSQL0005 | Usage | Warning | Use of attributes associated with obsolete SQL types (e.g., TEXT, IMAGE) that should be avoided in new development.
