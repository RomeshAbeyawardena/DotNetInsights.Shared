using System;
using System.Threading.Tasks;

namespace DotNetInsights.Shared.Contracts
{
    public interface IAppHost
    {
        object Run(string methodName);
        Task RunAsync(string methodName);
        Task<T> RunAsync<T>(string methodName);
    }

    public interface IAppHost<TStartup> : IAppHost
    {
        object Run(Func<TStartup, object> getMember);
        Task RunAsync(Func<TStartup, Task> getMemberTask);
        Task<T> RunAsync<T>(Func<TStartup, Task<T>> getMemberTask);
    }
}
