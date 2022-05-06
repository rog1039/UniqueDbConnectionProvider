using FluentAssertions;
using UniqueDb.ConnectionProvider.DataGeneration.SqlMetadata;

namespace UniqueDb.ConnectionProvider.DataGeneration;

public class InformationSchemaTableDefinition
{
    public InformationSchemaTable         InformationSchemaTable   { get; set; }
    public IList<InformationSchemaColumn> InformationSchemaColumns { get; set; }
    public IList<TableConstraintInfoDto>  TableConstraints         { get; set; }

    public IList<InformationSchemaColumn> GetPrimaryKeyColumns()
    {
        return InformationSchemaColumns
            .Where(IsColumnPrimaryKey)
            .ToList();
    }


    public bool IsColumnPrimaryKey(InformationSchemaColumn col)
    {
        if (col.TABLE_SCHEMA != InformationSchemaTable.TABLE_SCHEMA ||
            col.TABLE_NAME != InformationSchemaTable.TABLE_NAME)
        {
            var columnTableName = $"{col.TABLE_SCHEMA}.{col.TABLE_NAME}";
            var tableTableName  = $"{InformationSchemaTable.TABLE_SCHEMA}.{InformationSchemaTable.TABLE_NAME}";

            throw new Exception(
                $"Column and Table names do not match. Col: {columnTableName}; Table:{tableTableName}");
        }

        var columnName = col.COLUMN_NAME;
        return IsColumnPrimaryKey(columnName);
    }

    public bool IsColumnPrimaryKey(string columnName)
    {
        var isPrimaryKey = TableConstraints
            .Where(z => z.CONSTRAINT_TYPE == TableConstraintInfoDto.PrimaryKeyConstraintType)
            .Any(z => z.COLUMN_NAME.InsensitiveEquals(columnName));
        return isPrimaryKey;
    }
}

public class DbTableName3 : IEquatable<DbTableName3>
{
    public bool Equals(DbTableName3? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return string.Equals(Schema, other.Schema, StringComparison.OrdinalIgnoreCase) &&
               string.Equals(Name,   other.Name,   StringComparison.OrdinalIgnoreCase);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((DbTableName3) obj);
    }

    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add(Schema, StringComparer.OrdinalIgnoreCase);
        hashCode.Add(Name,   StringComparer.OrdinalIgnoreCase);
        return hashCode.ToHashCode();
    }

    public static bool operator ==(DbTableName3? left, DbTableName3? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(DbTableName3? left, DbTableName3? right)
    {
        return !Equals(left, right);
    }

    public string Schema { get; set; }
    public string Name   { get; set; }

    public DbTableName3() { }

    public DbTableName3(string fullName)
    {
        var names = fullName.Split(new[] {'.'}, StringSplitOptions.RemoveEmptyEntries);
        names.Length.Should().BeGreaterOrEqualTo(2);

        var schema = names[0];
        var table  = string.Join(".", names.Skip(1));
        Schema = schema.Debracketize();
        Name   = table.Debracketize();
    }

    public DbTableName3(string schema, string name)
    {
        Schema = schema.Debracketize();
        Name   = name.Debracketize();
    }

    public override string ToString()
    {
        return string.IsNullOrWhiteSpace(Schema)
            ? Name
            : $"[{Schema}].[{Name}]";
    }
}