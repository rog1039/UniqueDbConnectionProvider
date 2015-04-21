﻿namespace UniqueDb.ConnectionProvider
{
    public class UniqueDbConnectionProviderOptions
    {

        public UniqueDbConnectionProviderOptions(string sqlServerName, string databaseNamePrefix)
        {
            SqlServerName = sqlServerName;
            DatabaseNamePrefix = databaseNamePrefix;
            IncludeTimeStamp = true;
            TimeStampFormat = "yyMMdd.HHmmss.fff";
            UseIntegratedSecurity = true;
        }

        public UniqueDbConnectionProviderOptions(string sqlServerName, string databaseNamePrefix, bool includeTimeStamp, string timeStampFormat)
        {
            SqlServerName = sqlServerName;
            IncludeTimeStamp = includeTimeStamp;
            TimeStampFormat = timeStampFormat;
            DatabaseNamePrefix = databaseNamePrefix;
            UseIntegratedSecurity = true;
        }

        public UniqueDbConnectionProviderOptions(string sqlServerName, string databaseNamePrefix, string userName, string password)
            : base()
        {
            SqlServerName = sqlServerName;
            DatabaseNamePrefix = databaseNamePrefix;
            IncludeTimeStamp = true;
            TimeStampFormat = "yyMMdd.HHmmss.fff";
            UseIntegratedSecurity = false;
            UserName = userName;
            Password = password;
        }

        public UniqueDbConnectionProviderOptions(string sqlServerName, string databaseNamePrefix, bool includeTimeStamp, string timeStampFormat, string userName, string password)
        {
            IncludeTimeStamp = includeTimeStamp;
            TimeStampFormat = timeStampFormat;
            SqlServerName = sqlServerName;
            DatabaseNamePrefix = databaseNamePrefix;
            UseIntegratedSecurity = false;
            UserName = userName;
            Password = password;
        }

        public bool IncludeTimeStamp { get; private set; }
        public string TimeStampFormat { get; private set; }
        public string SqlServerName { get; private set; }
        public string DatabaseNamePrefix { get; private set; }
        public bool UseIntegratedSecurity { get; private set; }
        public string UserName { get; private set; }
        public string Password { get; private set; }
    }
}