namespace UniqueDb.ConnectionProvider.Tests.DataGeneration
{
    public class SqlTableColumnOverride
    {
        public string Table { get; set; }
        public string Column { get; set; }

        public string CSharpName { get; set; }
        public string CSharpType { get; set; }
    }
}