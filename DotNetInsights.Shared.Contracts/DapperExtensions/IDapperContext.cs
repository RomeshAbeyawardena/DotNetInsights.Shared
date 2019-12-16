using System;

namespace DotNetInsights.Shared.Contracts.DapperExtensions
{
    public interface IDapperContext : IDisposable
    {
        void MapContext(IDapperContext dapperContext = null);
    }
}