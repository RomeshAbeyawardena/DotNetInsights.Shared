using DotNetInsights.Shared.Domains;
using System;
using System.Threading.Tasks;

namespace DotNetInsights.Shared.Contracts
{
    public interface ISqlDependencyChangeEvent
    {
        Task OnChange(CommandEntrySqlNotificationEventArgs e);
    }

    public interface ISqlDependencyChangeEvent<T> 
        : ISqlDependencyChangeEvent
    {

    }
}
