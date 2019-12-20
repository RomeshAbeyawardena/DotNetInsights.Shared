using DotNetInsights.Shared.Domains;
using System.Threading.Tasks;

namespace DotNetInsights.Shared.Contracts.Services
{
    public interface ILoggingService
    {
        Task LogEntry(LogEntry logEntry);
    }
}