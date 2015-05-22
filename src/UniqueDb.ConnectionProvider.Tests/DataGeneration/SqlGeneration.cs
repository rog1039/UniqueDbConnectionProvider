﻿using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using UniqueDb.ConnectionProvider.DataGeneration;
using UniqueDb.ConnectionProvider.DataGeneration.CSharpGeneration;
using Xunit;

namespace UniqueDb.ConnectionProvider.Tests.DataGeneration
{

    public static class UniqueTestingDbProvider
    {
        public static UniqueDbConnectionProvider GetUniqueSqlConnectionProvider(
            string databasePrefix = "UniqueDb")
        {
            var dbConnectionProvider = new UniqueDbConnectionProvider(
                new UniqueDbConnectionProviderOptions("ws2012sqlexp1\\sqlexpress", databasePrefix))
                .AndAutoDeleteDbOlderThan5Minutes();
            return dbConnectionProvider;
        }
    }

    public class TestingConversionFromDescribeResultSetToSqlDataTypes
    {
        private static void RunTestCase(SqlTypeColumnTestCase testCase)
        {
            var tableGenerationScript = $"create table TestTable( FirstName {testCase.ColumnDeclaration});";
            var uniqueDbProvider = UniqueTestingDbProvider.GetUniqueSqlConnectionProvider();
            using (uniqueDbProvider.ToDisposable())
            {
                uniqueDbProvider.CreateDatabase();
                testCase.PreTestSqlScripts.ForEach(uniqueDbProvider.Execute);
                uniqueDbProvider.Execute(tableGenerationScript);

                var query = "select * from TestTable";
                var containers = CSharpClassGeneratorFromQueryViaSqlDescribeResultSet.GetDescribeResultSetContainers(
                    uniqueDbProvider, query);

                var sqlColumns = containers.Select(DescribeResultSetRowToSqlColumnConverter.Convert);
                var column = sqlColumns.First();

                column.SqlDataType.TypeName.Should().Be(testCase.ExpectedSqlType.TypeName);
                column.SqlDataType.Mantissa.Should().Be(testCase.ExpectedSqlType.Mantissa);
                column.SqlDataType.MaximumCharLength.Should().Be(testCase.ExpectedSqlType.MaximumCharLength);
                column.SqlDataType.NumericScale.Should().Be(testCase.ExpectedSqlType.NumericScale);
                column.SqlDataType.NumericPrecision.Should().Be(testCase.ExpectedSqlType.NumericPrecision);
                column.SqlDataType.TypeName.Should().Be(testCase.ExpectedSqlType.TypeName);
                column.SqlDataType.FractionalSecondsPrecision.Should()
                    .Be(testCase.ExpectedSqlType.FractionalSecondsPrecision);
            }
        }


        [Fact()]
        [Trait("Category", "Integration")]
        public void Int()
        {
            var testCase = new SqlTypeColumnTestCase()
            {
                ColumnDeclaration = "int",
                ExpectedSqlType = new SqlType()
                {
                    TypeName = "int",
                }
            };
            RunTestCase(testCase);
        }

        [Fact()]
        [Trait("Category", "Integration")]
        public void DecimalDefault()
        {
            var testCase = new SqlTypeColumnTestCase()
            {
                ColumnDeclaration = "decimal",
                ExpectedSqlType = new SqlType()
                {
                    TypeName = "decimal",
                    NumericPrecision = 18,
                    NumericScale = 0
                }
            };
            RunTestCase(testCase);
        }

        [Fact()]
        [Trait("Category", "Integration")]
        public void DecimalPrecision()
        {
            var testCase = new SqlTypeColumnTestCase()
            {
                ColumnDeclaration = "decimal(25)",
                ExpectedSqlType = new SqlType()
                {
                    TypeName = "decimal",
                    NumericPrecision = 25,
                    NumericScale = 0
                }
            };
            RunTestCase(testCase);
        }

        [Fact()]
        [Trait("Category", "Integration")]
        public void DecimalPrecisionAndScale()
        {
            var testCase = new SqlTypeColumnTestCase()
            {
                ColumnDeclaration = "decimal(30,5)",
                ExpectedSqlType = new SqlType()
                {
                    TypeName = "decimal",
                    NumericPrecision = 30,
                    NumericScale = 5
                }
            };
            RunTestCase(testCase);
        }

        [Fact()]
        [Trait("Category", "Integration")]
        public void Datetime()
        {
            var testCase = new SqlTypeColumnTestCase()
            {
                ColumnDeclaration = "datetime",
                ExpectedSqlType = new SqlType()
                {
                    TypeName = "datetime",
                }
            };
            RunTestCase(testCase);
        }

        [Fact()]
        [Trait("Category", "Integration")]
        public void Datetime2()
        {
            var testCase = new SqlTypeColumnTestCase()
            {
                ColumnDeclaration = "datetime2",
                ExpectedSqlType = new SqlType()
                {
                    TypeName = "datetime2",
                    FractionalSecondsPrecision = 7
                }
            };
            RunTestCase(testCase);
        }

        [Fact()]
        [Trait("Category", "Integration")]
        public void Datetime2WithPrecision()
        {
            var testCase = new SqlTypeColumnTestCase()
            {
                ColumnDeclaration = "datetime2(4)",
                ExpectedSqlType = new SqlType()
                {
                    TypeName = "datetime2",
                    FractionalSecondsPrecision = 4
                }
            };
            RunTestCase(testCase);
        }

        [Fact()]
        [Trait("Category", "Integration")]
        public void UserDefinedType()
        {
            var testCase = new SqlTypeColumnTestCase()
            {
                ColumnDeclaration = "mytypespecial",
                ExpectedSqlType = new SqlType()
                {
                    TypeName = "nvarchar",
                    MaximumCharLength = 20
                }
            };
            testCase.PreTestSqlScripts.Add("create type mytypespecial from nvarchar(10);");
            RunTestCase(testCase);
        }

        [Fact()]
        [Trait("Category", "Integration")]
        public void Nvarchar()
        {
            var testCase = new SqlTypeColumnTestCase()
            {
                ColumnDeclaration = "nvarchar(10)",
                ExpectedSqlType = new SqlType()
                {
                    TypeName = "nvarchar",
                    MaximumCharLength = 20
                }
            };
            RunTestCase(testCase);
        }

        [Fact()]
        [Trait("Category", "Integration")]
        public void Varchar()
        {
            var testCase = new SqlTypeColumnTestCase()
            {
                ColumnDeclaration = "varchar(10)",
                ExpectedSqlType = new SqlType()
                {
                    TypeName = "varchar",
                    MaximumCharLength = 10
                }
            };
            RunTestCase(testCase);
        }
    }

    public class SqlTypeColumnTestCase
    {
        public string ColumnDeclaration { get; set; }
        public SqlType ExpectedSqlType { get; set; }
        public List<string> PreTestSqlScripts { get; set; } = new List<string>(); 
    }
}
