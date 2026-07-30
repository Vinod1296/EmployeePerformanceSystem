/* 
    Pre-drop dependency check for Employees.Role

    This script lists objects that reference Employees.Role through module definitions
    and dependency metadata before attempting the column drop.
*/

SET NOCOUNT ON;

DECLARE @SchemaName sysname = N'dbo';
DECLARE @TableName sysname = N'Employees';
DECLARE @ColumnName sysname = N'Role';
DECLARE @FullName nvarchar(300) = QUOTENAME(@SchemaName) + N'.' + QUOTENAME(@TableName);

PRINT N'Checking SQL Server dependencies for ' + @FullName + N'.' + QUOTENAME(@ColumnName) + N'...';

SELECT
    referencing_schema_name = OBJECT_SCHEMA_NAME(d.referencing_id),
    referencing_object_name = OBJECT_NAME(d.referencing_id),
    referencing_object_type = o.type_desc,
    referenced_schema_name = d.referenced_schema_name,
    referenced_entity_name = d.referenced_entity_name,
    referenced_minor_name = d.referenced_minor_name
FROM sys.sql_expression_dependencies AS d
INNER JOIN sys.objects AS o
    ON o.object_id = d.referencing_id
WHERE d.referenced_id = OBJECT_ID(@FullName)
  AND d.referenced_minor_name = @ColumnName
ORDER BY referencing_schema_name, referencing_object_name;

SELECT
    object_schema = SCHEMA_NAME(o.schema_id),
    object_name = o.name,
    object_type = o.type_desc
FROM sys.sql_modules AS m
INNER JOIN sys.objects AS o
    ON o.object_id = m.object_id
WHERE m.definition LIKE N'%Employees%'
  AND m.definition LIKE N'%Role%'
ORDER BY object_schema, object_name;

IF EXISTS (
    SELECT 1
    FROM sys.sql_expression_dependencies AS d
    WHERE d.referenced_id = OBJECT_ID(@FullName)
      AND d.referenced_minor_name = @ColumnName
)
BEGIN
    THROW 50000, 'Dependencies still exist for Employees.Role. Review the result sets above before dropping the column.', 1;
END;

PRINT N'No dependency rows found. Proceeding with column drop.';
ALTER TABLE dbo.Employees
DROP COLUMN Role;
