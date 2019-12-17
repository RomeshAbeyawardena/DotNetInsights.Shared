using Microsoft.Data.SqlClient;
using System;

namespace DotNetInsights.Shared.Domains
{
    public sealed class CommandEntry : IDisposable
    {
        private CommandEntry(string name, string command, Type entityType)
        {
            Name = name;
            Command = command;
            EntityType = entityType;
        }

        public static CommandEntry Create(string name, string command, Type entityType)
        {
            return new CommandEntry(name, command, entityType);
        }


        public void Dispose()
        {
            DbCommand?.Dispose();
        }

        public string Name { get; }
        public string Command { get; }
        public Type EntityType { get; }
        public SqlCommand DbCommand { get; set; }
        public SqlDependency SqlDependency { get; set; }
    }
}