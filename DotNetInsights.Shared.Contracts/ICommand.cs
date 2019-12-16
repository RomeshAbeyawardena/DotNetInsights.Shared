using System.Collections.Generic;

namespace DotNetInsights.Shared.Contracts
{
    public interface ICommand
    {
        string Name { get; set; }
        IDictionary<string, object> Parameters { get; }
    }
}
