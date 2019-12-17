using Microsoft.Data.SqlClient;
using DotNetInsights.Shared.Contracts;
using DotNetInsights.Shared.Domains;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.Threading;
using System.Data;

namespace DotNetInsights.Shared.Services
{
    public sealed class DefaultSqlDependencyManager : ISqlDependencyManager
    {
        public event EventHandler<CommandEntrySqlNotificationEventArgs> OnChange;

        public IDictionary<string, CommandEntry> CommandEntries
        {
            get
            {
                var dictionary = new Dictionary<string, CommandEntry>();

                foreach (var commandEntry in _commandEntries)
                {
                    if (commandEntry.SqlDependency == null)
                        throw new NotSupportedException("SqlDependencyManager not started!");
                    dictionary.Add(commandEntry.SqlDependency.Id, commandEntry);
                }

                return dictionary;
            }
        }

        public ISqlDependencyManager AddCommandEntry(string name, string command, Type entityType)
        {
            AddCommandEntry(CommandEntry.Create(name, command, entityType));
            return this;
        }

        public ISqlDependencyManager AddCommandEntry(CommandEntry commandEntry)
        {
            _commandEntries.Add(commandEntry);
            return this;
        }

        public void Dispose()
        {
            Dispose(true);

        }

        public async Task Start(string connectionString)
        {
            SqlDependency.Start(connectionString);
            _sqlConnection = new SqlConnection(connectionString);
        }

        public void Stop(string connectionString)
        {
            SqlDependency.Stop(connectionString);
            End();
        }


        private void Dispose(bool gc)
        {
            End();
            _sqlConnection?.Dispose();
        }


        public async Task Listen()
        {
            _isTriggered = false;

            if(_sqlConnection.State != ConnectionState.Open)
                await _sqlConnection.OpenAsync()
                    .ConfigureAwait(false);

            for (var entryIndex = 0; entryIndex < _commandEntries.Count; entryIndex++)
            {
                var entry = _commandEntries[entryIndex];
                entry.SqlDependency = await CreateSqlDependency(entry)
                    .ConfigureAwait(false);
            }

            await IsTriggered();

            await _sqlConnection.CloseAsync();
        }

        private void End()
        {
            foreach (var entry in _commandEntries)
            {
                entry?.Dispose();
            }
        }

        private async Task<SqlDependency> CreateSqlDependency(CommandEntry commandEntry)
        {
            var sqlDependency = new SqlDependency();
            sqlDependency.OnChange += SqlDependency_OnChange;

            commandEntry.DbCommand = new SqlCommand(commandEntry.Command, _sqlConnection);
            sqlDependency.AddCommandDependency(commandEntry.DbCommand);
            await commandEntry.DbCommand.ExecuteReaderAsync()
                .ConfigureAwait(false);
            
            return sqlDependency;
        }

        private void SqlDependency_OnChange(object sender, SqlNotificationEventArgs e)
        {
            if (!(sender is SqlDependency sqlDependency))
                throw new InvalidOperationException();

            var commandEntries = from commandEntry in _commandEntries
                                 where commandEntry.SqlDependency.Id == sqlDependency.Id
                                 select commandEntry;
            
            OnChange?.Invoke(this, new CommandEntrySqlNotificationEventArgs(commandEntries.FirstOrDefault(), e.Type, e.Info, e.Source));

            _isTriggered = true;
        }

        private async Task IsTriggered()
        {
            while(!_isTriggered)
                await Task.Delay(1000);
        }

        public DefaultSqlDependencyManager()
        {
            _commandEntries = new List<CommandEntry>();
        }

        private bool _isTriggered = false;
        private readonly IList<CommandEntry> _commandEntries;
        private SqlConnection _sqlConnection;
    }
}