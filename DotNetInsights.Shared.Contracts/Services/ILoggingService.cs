using DotNetInsights.Shared.Domains;
using System;
using System.Threading.Tasks;

namespace DotNetInsights.Shared.Contracts.Services
{
    public interface ILoggingService : IDisposable
    {
        Task<int> LogEntry(LogEntry logEntry);
    }
}