using DotNetInsights.Shared.Contracts;
using DotNetInsights.Shared.Domains;

namespace DotNetInsights.Shared.Services
{
    public sealed class SqlDependencyChangeEventQueueItem
    {
        private SqlDependencyChangeEventQueueItem(ISqlDependencyChangeEvent sqlDependencyChangeEvent,
            CommandEntrySqlNotificationEventArgs commandEntry)
        {
            SqlDependencyChangeEvent = sqlDependencyChangeEvent;
            CommandEntry = commandEntry;
        }

        public static SqlDependencyChangeEventQueueItem Create(ISqlDependencyChangeEvent sqlDependencyChangeEvent,
            CommandEntrySqlNotificationEventArgs commandEntry)
        {
            return new SqlDependencyChangeEventQueueItem(sqlDependencyChangeEvent, commandEntry);
        }

        public ISqlDependencyChangeEvent SqlDependencyChangeEvent { get; }
        public CommandEntrySqlNotificationEventArgs CommandEntry { get; }
    }
}