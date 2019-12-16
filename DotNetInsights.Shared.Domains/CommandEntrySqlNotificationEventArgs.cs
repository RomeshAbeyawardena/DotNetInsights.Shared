using Microsoft.Data.SqlClient;

namespace DotNetInsights.Shared.Domains
{
    public class CommandEntrySqlNotificationEventArgs : SqlNotificationEventArgs
    {
        public CommandEntrySqlNotificationEventArgs(CommandEntry commandEntry, SqlNotificationType type, SqlNotificationInfo info, SqlNotificationSource source) 
            : base(type, info, source)
        {
            CommandEntry = commandEntry;
        }

        public CommandEntrySqlNotificationEventArgs(CommandEntry commandEntry, SqlNotificationEventArgs eventArgs)
            : this(commandEntry, eventArgs.Type, eventArgs.Info, eventArgs.Source)
        {

        }

        public CommandEntry CommandEntry { get; }
    }
}