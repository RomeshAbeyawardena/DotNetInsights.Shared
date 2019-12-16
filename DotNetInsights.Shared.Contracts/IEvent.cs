using System;
using System.Collections.Generic;

namespace DotNetInsights.Shared.Contracts
{
    public interface IEvent
    {
        object Result { get;  }
        IEnumerable<object> Results { get; }
        bool IsSuccessful { get; }
        Exception Exception { get; }
    }

    public interface IEvent<TResult> : IEvent
        where TResult : class
    {
        new TResult Result { get; }
        new IEnumerable<TResult> Results { get; }
    }
}
