using Microsoft.Data.SqlClient;
using System;

namespace DotNetInsights.Shared.Domains
{
    public sealed class CommandEntry : IDisposable
    {
        private CommandEntry(string name, string command, SqlDependency sqlDependency = null)
        {
            Name = name;
            Command = command;
            if(sqlDependency != null)
                SqlDependency = sqlDependency;
        }

        public static CommandEntry Create(string name, string command)
        {
            return new CommandEntry(name, command);
        }

        public static CommandEntry Create(string name, string command, SqlDependency sqlDependency)
        {
            return new CommandEntry(name, command, sqlDependency);
        }

        public void Dispose()
        {
            DbCommand?.Dispose();
        }

        public string Name { get; }
        public string Command { get; }
        public SqlCommand DbCommand { get; set; }
        public SqlDependency SqlDependency { get; set; }
    }
}