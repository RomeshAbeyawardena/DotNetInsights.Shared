using System;
using System.Collections.Generic;
using System.Linq;
using DotNetInsights.Shared.Contracts;

namespace DotNetInsights.Shared.Domains
{
    public static class DefaultEvent
    {
        public static IEvent<T> Create<T>(T result = null, IEnumerable<T> results = null)
            where T : class
        {
            return new DefaultEvent<T>(result, results);
        }

        public static IEvent<T> Create<T>(bool isSuccessful, Exception exception = null, T result = null, IEnumerable<T> results = null)
            where T : class
        {
            return new DefaultEvent<T>(isSuccessful, exception, result, results);
        }
    }

    public class DefaultEvent<T> : IEvent<T>
        where T : class
    {
        public DefaultEvent(T result = null, IEnumerable<T> results = null)
        {
            Result = result;
            Results = results;
            IsSuccessful = true;
        }

        public DefaultEvent(bool isSuccessful, Exception exception = null, T result = null, IEnumerable<T> results = null)
        {
            Results = results;
            Result = result;
            IsSuccessful = isSuccessful;
            Exception = exception;
        }
        public IEnumerable<T> Results { get; }
        public T Result { get; }
        public bool IsSuccessful { get; }
        public Exception Exception { get; }

        object IEvent.Result { get => Result; }

        IEnumerable<object> IEvent.Results => Results.Select(result => (object)result);
    }
}
