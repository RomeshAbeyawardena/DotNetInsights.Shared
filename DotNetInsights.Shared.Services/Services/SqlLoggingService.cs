using DotNetInsights.Shared.Contracts.Services;
using DotNetInsights.Shared.Domains;
using DotNetInsights.Shared.Library.Extensions;
using DotNetInsights.Shared.Library.Options;
using Microsoft.Data.SqlClient;
using System;
using System.Threading.Tasks;

namespace DotNetInsights.Shared.Services
{
    public class SqlLoggingService : ILoggingService
    {
        private readonly SqlConnection _sqlConnection;
        private readonly string _tableSchema;
        private readonly string _tableName;
        

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool gc)
        {
            _sqlConnection.Dispose();
        }

        public async Task<int> LogEntry(LogEntry logEntry)
        {
            return await ConsumeSqlCommand(string.Format("INSERT INTO [{0}].[{1}] ([Category], [LogLevelId], [EventId], [EventName], [FormattedString], [Created])",
                _tableSchema, _tableName) + " VALUES (@category, @logLevelId, @eventId, @eventName, @formattedString, @created)",
                parameters => parameters.Add("category", logEntry.Category)
                    .Add("logLevelId", (int)logEntry.LogLevelId)
                    .Add("eventId", logEntry.EventId)
                    .Add("eventName", logEntry.EventName)
                    .Add("formattedString", logEntry.FormattedString)
                    .Add("created", logEntry.Created), async(sqlCommand) => await sqlCommand.ExecuteNonQueryAsync());
        }

        private async Task<T> ConsumeSqlCommand<T>(string command, Action<SqlParameterCollection> getParameters,  Func<SqlCommand, Task<T>> getCommand)
        {
            T returnValue = default;
            _sqlConnection.Open();
            using (var sqlCommand = new SqlCommand(command, _sqlConnection))
            {
                getParameters(sqlCommand.Parameters);
                
                returnValue = await getCommand(sqlCommand);
            }
            _sqlConnection.Close();
            return returnValue;
        }

        public SqlLoggingService(IServiceProvider serviceProvider, SqlLoggerOptions sqlLoggingOptions)
        {
            _sqlConnection = new SqlConnection(sqlLoggingOptions.GetConnectionString(serviceProvider));
            _tableSchema = sqlLoggingOptions.GetTableSchema(serviceProvider);
            _tableName = sqlLoggingOptions.GetTableName(serviceProvider);
        }
    }
}