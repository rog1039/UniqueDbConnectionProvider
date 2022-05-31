using UniqueDb.ConnectionProvider.Infrastructure.Extensions;

namespace UniqueDb.ConnectionProvider.DataGeneration.SqlMetadata;

public class PkAnnotation
{
   public DbTableName   DbTableName   { get; set; }
   public IList<string> PkColumnNames { get; set; }

   public PkAnnotation(string tableName, string columns)
   {
      DbTableName   = tableName;
      PkColumnNames = columns.SplitOn(',');
   }
}