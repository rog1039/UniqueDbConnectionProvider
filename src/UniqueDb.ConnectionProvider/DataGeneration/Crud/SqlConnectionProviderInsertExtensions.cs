﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Xml;

namespace UniqueDb.ConnectionProvider.DataGeneration.Crud
{
    public static class SqlConnectionProviderUpdateExtensions
    {
        public static void Update<T>(this ISqlConnectionProvider sqlConnectionProvider, T objectToUpdate,
            Expression<Func<T, object>> keyProperties = null, string tableName = null, string schemaName = null)
        {
            tableName = SqlTextFunctions.GetTableName(objectToUpdate, tableName, schemaName);

            using (var myConnection = sqlConnectionProvider.GetSqlConnection())
            {
                using (var myCommand = new SqlCommand() {Connection = myConnection})
                {
                    var setClauseProperties = SqlTextFunctions.GetRelevantPropertyInfos(objectToUpdate, null);
                    var whereClauseProperties = SqlTextFunctions.GetPropertiesFromObject(objectToUpdate, keyProperties);
                    BuildOutUpdateCommand(objectToUpdate, tableName, setClauseProperties, whereClauseProperties, myCommand);

                    myConnection.Open();
                    myCommand.ExecuteNonQuery();
                    myConnection.Close();
                }
            }
        }

        private static void BuildOutUpdateCommand(object objectToUpdate, string tableName, IList<PropertyInfo> setClauseProperties, IList<PropertyInfo> whereClauseProperties, SqlCommand myCommand)
        {
            var setClauseColumnNames = setClauseProperties.Select(SqlTextFunctions.GetColumnNameFromPropertyInfo).ToList();
            var setClauseParameterNames = setClauseProperties.Select(SqlTextFunctions.GetSetClauseParameterName).ToList();
            var setClauseParts = Enumerable
                .Range(0, setClauseColumnNames.Count)
                .Select(index => $"{setClauseColumnNames[index]} = {setClauseParameterNames[index]}");
            var setClause = string.Join(", ", setClauseParts);

            var whereClauseColumnNames = whereClauseProperties.Select(SqlTextFunctions.GetColumnNameFromPropertyInfo).ToList();
            var whereClauseParameterNames = whereClauseProperties.Select(SqlTextFunctions.GetWhereClauseParameterName).ToList();
            var whereClauseParts = Enumerable
                .Range(0, whereClauseColumnNames.Count())
                .Select(index => $"{whereClauseColumnNames[index]} = {whereClauseParameterNames[index]}");
            var whereClause = string.Join(" AND ", whereClauseParts);

            var setClauseParameters = Enumerable
                .Range(0, setClauseProperties.Count)
                .Select(index => SqlTextFunctions.GetParameter(objectToUpdate, setClauseProperties[index], setClauseParameterNames[index]))
                .ToList();
            var whereClauseParameters = Enumerable
                .Range(0, whereClauseProperties.Count)
                .Select(index => SqlTextFunctions.GetParameter(objectToUpdate, whereClauseProperties[index], whereClauseParameterNames[index]))
                .ToList();

            
            myCommand.Parameters.AddRange(setClauseParameters.ToArray());
            myCommand.Parameters.AddRange(whereClauseParameters.ToArray());
            myCommand.CommandText = $"UPDATE {tableName} SET {setClause} WHERE {whereClause}";
        }
    }

    public static class SqlConnectionProviderInsertExtensions
    {
        public static void Insert(this ISqlConnectionProvider sqlConnectionProvider, object obj, string tableName = null, string schemaName = null, IEnumerable<string> columnsToIgnore = null)
        {
            tableName = SqlTextFunctions.GetTableName(obj, tableName, schemaName);

            var propertyInfos = SqlTextFunctions.GetRelevantPropertyInfos(obj, columnsToIgnore);
            if (propertyInfos.Count == 0)
                return;

            using (var myConnection = sqlConnectionProvider.GetSqlConnection())
            {
                using (var myCommand = new SqlCommand() {Connection = myConnection})
                {
                    BuildOutMyCommand(obj, tableName, propertyInfos, myCommand);

                    myConnection.Open();
                    myCommand.ExecuteNonQuery();
                    myConnection.Close();
                }
            }
        }

        private static void BuildOutMyCommand(object obj, string tableName, IList<PropertyInfo> propertyInfos, SqlCommand myCommand)
        {
            var columnList = string.Join(", ", propertyInfos.Select(SqlTextFunctions.GetColumnNameFromPropertyInfo));
            var sqlParameterNames = string.Join(", ", propertyInfos.Select(SqlTextFunctions.GetParameterName));
            var sqlParameters = propertyInfos.Select(pi => SqlTextFunctions.GetParameter(obj, pi)).ToList();

            myCommand.Parameters.AddRange(sqlParameters.ToArray());
            myCommand.CommandText = $"INSERT INTO {tableName} ({columnList}) values ({sqlParameterNames})";
        }
    }
}
