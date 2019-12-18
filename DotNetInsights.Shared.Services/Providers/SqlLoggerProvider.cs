using DotNetInsights.Shared.Library.Extensions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetInsights.Shared.Services.Providers
{
    public class SqlLoggerProvider : ILoggerProvider
    {
        public ILogger CreateLogger(string categoryName)
        {
            return new SqlLogger(categoryName, _sqlLogWriter);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool gc)
        {
            _sqlLogWriter.Dispose();
        }

        public SqlLoggerProvider(IServiceProvider serviceProvider, SqlLoggerOptions sqlLoggerOptions)
        {
            var connectionString = sqlLoggerOptions.GetConnectionString(serviceProvider);
            _sqlLogWriter = new SqlLogWriter(connectionString, 
                sqlLoggerOptions.GetTableName(serviceProvider), 
                sqlLoggerOptions.GetTableSchema(serviceProvider));
        }

        private readonly SqlLogWriter _sqlLogWriter;
    }

    internal class SqlLogger : ILogger
    {
        private string _categoryName;
        private SqlLogWriter _sqlLogWriter;
        private object _state;
        public IDisposable BeginScope<TState>(TState state)
        {
            return _sqlLogWriter;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return _sqlLogWriter.IsEnabled(logLevel);
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            _sqlLogWriter.WriteEntry(logLevel, eventId, formatter(state, exception), DateTime.Now);
        }

        public SqlLogger(string categoryName, SqlLogWriter sqlLogWriter)
        {
            _categoryName = categoryName;
            _sqlLogWriter = sqlLogWriter;
            _sqlLogWriter.CategoryName = _categoryName;
        }
    }

    internal class SqlLogWriter : IDisposable
    {
        public void Dispose()
        {
            Dispose(true);
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void WriteEntry(LogLevel logLevel, EventId eventId, string formattedString, DateTime createdDateTime)
        {
            _sqlConnection.Open();
            var insertDefinition = string.Format("INSERT INTO [{0}].[{1}] ([Category], [LogLevelId], [EventId], [EventName], [FormattedString], [Created])",
                _tableSchema, _tableName);
            using (var sqlCommand = new SqlCommand(insertDefinition + " VALUES (@category, @logLevelId, @eventId, @eventName, @formattedString, @created)", _sqlConnection))
            {
                sqlCommand.Parameters
                    .Add("category", CategoryName)
                    .Add("logLevelId", (int)logLevel)
                    .Add("eventId", eventId.Id)
                    .Add("eventName", eventId.Name)
                    .Add("formattedString", formattedString)
                    .Add("created", createdDateTime);

                sqlCommand.ExecuteNonQuery();
            }

            _sqlConnection.Close();
        }

        public string CategoryName { get; set; }

        protected virtual void Dispose(bool gc)
        {
            _sqlConnection.Dispose();
        }

        public SqlLogWriter(string connectionString, string tableName, string tableSchema = "dbo")
        {
            _sqlConnection = new SqlConnection(connectionString);
            _tableName = tableName;
            _tableSchema = tableSchema;
        }

        private readonly string _tableName;
        private readonly string _tableSchema;
        private readonly SqlConnection _sqlConnection;
    }
}
