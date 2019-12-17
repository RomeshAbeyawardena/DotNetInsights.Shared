using DotNetInsights.Shared.Contracts;
using DotNetInsights.Shared.Domains;
using Microsoft.Data.SqlClient;
using System;
using System.Threading.Tasks;

namespace DotNetInsights.Shared.Services
{
    public abstract class DefaultSqlDependencyChangeEvent : ISqlDependencyChangeEvent
    {
        public abstract Task OnChange(CommandEntrySqlNotificationEventArgs e);

        public bool IsDataChange(CommandEntrySqlNotificationEventArgs e) => e.Type == SqlNotificationType.Change
                && e.Source == SqlNotificationSource.Data 
                && (e.Info == SqlNotificationInfo.Insert 
                     || e.Info == SqlNotificationInfo.Update
                     || e.Info == SqlNotificationInfo.Delete); 
    }
}
