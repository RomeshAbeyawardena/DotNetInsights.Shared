using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace DotNetInsights.Shared.Contracts.Services
{
    public interface IDefinedServiceScope : IDisposable
    {
        IServiceScope ServiceScope { get; }
    }
}