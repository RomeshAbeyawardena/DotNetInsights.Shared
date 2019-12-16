using DotNetInsights.Shared.Domains;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DotNetInsights.Shared.Contracts
{
    public interface ISqlDependencyManager : IDisposable
    {
        event EventHandler<CommandEntrySqlNotificationEventArgs> OnChange;
        ISqlDependencyManager AddCommandEntry(string name, string command);
        ISqlDependencyManager AddCommandEntry(CommandEntry commandEntry);
        Task Start(string connectionString);
        void Stop(string connectionString);
        Task Listen();
        IDictionary<string, CommandEntry> CommandEntries { get; }
    }
}