using DotNetInsights.Shared.Library.Extensions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System;
using System.Data;
using Microsoft.Extensions.DependencyInjection;
using DotNetInsights.Shared.Library.Options;
using DotNetInsights.Shared.Contracts.Services;
using DotNetInsights.Shared.Domains;
using DotNetInsights.Shared.Library;
using System.Collections.Concurrent;

namespace DotNetInsights.Shared.Services.Providers
{
    public class SqlLoggerProvider : ILoggerProvider
    {
        public ILogger CreateLogger(string categoryName)
        {
            return new SqlLogger(_serviceProvider, categoryName);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool gc)
        {
            
        }

        public SqlLoggerProvider(IServiceProvider serviceProvider, SqlLoggerOptions sqlLoggerOptions)
        {
            _serviceProvider = serviceProvider;
            _connectionString = sqlLoggerOptions.GetConnectionString(serviceProvider);
            _tableName = sqlLoggerOptions.GetTableName(serviceProvider);
            _logOptionsTableName = sqlLoggerOptions.GetLogOptionsTableName(serviceProvider);
            _tableSchema = sqlLoggerOptions.GetTableSchema(serviceProvider);
        }

        private ILoggingService _loggingService;
        private readonly IServiceProvider _serviceProvider;
        private readonly string _connectionString;
        private readonly string _tableName;
        private readonly string _logOptionsTableName;
        private readonly string _tableSchema;
    }

    internal class SqlLogger : ILogger
    {
        private string _categoryName;
        private IServiceProvider _serviceProvider;
        private object _state;
        public IDisposable BeginScope<TState>(TState state)
        {
            _serviceScope = _serviceProvider.CreateScope();
            return _serviceScope;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            var loggingQueue = _serviceProvider.GetRequiredService<ConcurrentQueue<SqlLoggerQueueItem>>();
            loggingQueue.Enqueue(new SqlLoggerQueueItem { 
                Category = _categoryName,
                LogLevelId = (int) logLevel, 
                EventId = eventId.Id, 
                FormattedString = formatter(state, exception), 
                Created = DateTime.Now });
        }

        public SqlLogger(IServiceProvider serviceProvider, string categoryName)
        {
            _categoryName = categoryName;
            _serviceProvider = serviceProvider;
        }
        private ILoggingService _loggingService;
        private IServiceScope _serviceScope;
    }

    internal class SqlLogWriter : IDisposable
    {
        public void Dispose()
        {
            Dispose(true);
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            var result = ConsumeSqlCommand(string.Format("SELECT [IsEnabled] FROM [{0}].[{1}] WHERE [LogLevelId] = @logLevelId", _tableSchema, _logOptionsTable),
                sqlParameter => sqlParameter.Add("logLevelId", (int)logLevel), sqlCommand => sqlCommand.ExecuteScalar());

            if(result == DBNull.Value)
                return true;

            return Convert.ToBoolean(result);
        }

        public void WriteEntry(LogLevel logLevel, EventId eventId, string formattedString, DateTime createdDateTime)
        {
            
            var insertDefinition = string.Format("INSERT INTO [{0}].[{1}] ([Category], [LogLevelId], [EventId], [EventName], [FormattedString], [Created])",
                _tableSchema, _tableName) + " VALUES (@category, @logLevelId, @eventId, @eventName, @formattedString, @created)";

            ConsumeSqlCommand(insertDefinition, parameters => parameters.Add("category", CategoryName)
                    .Add("logLevelId", (int)logLevel)
                    .Add("eventId", eventId.Id)
                    .Add("eventName", eventId.Name)
                    .Add("formattedString", formattedString)
                    .Add("created", createdDateTime), sqlCommand => sqlCommand.ExecuteNonQuery());

        }

        private T ConsumeSqlCommand<T>(string command, Action<SqlParameterCollection> getParameters,  Func<SqlCommand, T> getCommand)
        {
            T returnValue = default;
            _sqlConnection.Open();
            using (var sqlCommand = new SqlCommand(command, _sqlConnection))
            {
                getParameters(sqlCommand.Parameters);
                
                returnValue = getCommand(sqlCommand);
            }
            _sqlConnection.Close();
            return returnValue;
        }

        public string CategoryName { get; set; }

        protected virtual void Dispose(bool gc)
        {
            _sqlConnection.Dispose();
        }

        public SqlLogWriter(string connectionString, string tableName, string logOptionsTable, string tableSchema = "dbo")
        {
            _connectionString = connectionString;
            _sqlConnection = new SqlConnection(_connectionString);
            _logOptionsTable = logOptionsTable;
            _tableName = tableName;
            _tableSchema = tableSchema;
        }
        private readonly string _connectionString;
        private readonly string _logOptionsTable;
        private readonly string _tableName;
        private readonly string _tableSchema;
        private readonly SqlConnection _sqlConnection;
    }
}
