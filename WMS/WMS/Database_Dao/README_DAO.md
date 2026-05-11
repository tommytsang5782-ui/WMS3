# DAO Layer Conventions (unified naming and style)

## 0. FAQ

- **Is it correct for each DAO to use `Dao_Connection` + `OpenSQLConnection()`?**  
  Yes. Getting a connection per method and closing it when done is standard. Always close in **try/finally** (or use `DaoBase`’s `EnsureOpen`/`EnsureClose`) so connections are released on exception.

- **Should DAOs only expose Insert, Select, Update, Delete?**  
  These four are the main operations. Keep a few extra methods only when needed (e.g. `SelectUser_timestamp`, `SyncItem`, or composite operations in `Dao`). Remove or refactor the rest over time.

## 1. Naming (in place)

- **Select**: Query; returns `List<T>` or single row. Main query method is `Select`; keep names like `SelectXxx_timestamp` for timestamp-based queries.
- **Insert**: Add row(s); returns `int` (rows affected).
- **Update**: Update; returns `int`. Parameters follow entity design (single key or updateFrom/updateTo).
- **Delete**: Delete; returns `int`. Parameter is entity or key.

## 2. Connection and resources

- In each method: `OpenSQLConnection()` → run SQL → `Close()` in **finally**.
- **Base class**: `DaoBase` provides `EnsureOpen()` / `EnsureClose()` and `ExecuteNonQuery(sql, params)`; subclasses can inherit (see `Dao_Company`).
- **Already using try/finally + parameters**: `Dao_Company` (inherits DaoBase), `Dao_Setup`, `Dao_User`, `Dao_Item`; others can follow.

## 3. SQL style

- **All new or updated methods** use **parameterized queries** (`SqlParameter` / `AddWithValue`) to avoid injection and quoting issues.
- Use `[]` for identifiers (e.g. `[Document No_]`); pass values only via parameters.

## 4. Error handling

- Use try/catch in DAOs as needed for logging or to let the caller return API errors.

## 5. Status and architecture

- **Connection config**: `DbConnectionConfig.Set(...)` is called at startup; `Dao_Connection` no longer depends on Form1.
- **DaoManager**: Moved to `WMS.Database_Dao.DaoManager` as the single entry point for DAOs.
- **Base class**: `DaoBase` is abstract with `EnsureOpen` / `EnsureClose` / `ExecuteNonQuery`; `Dao_Company` inherits it.
- **CRUD**: Completed for Dao_Synchronize, Dao_ScannedPackingHeader/Line/Mapping, etc.
- **Style example**: `Dao_Company`, `Dao_Setup`, `Dao_User`, `Dao_Item` use parameters and try/finally.
