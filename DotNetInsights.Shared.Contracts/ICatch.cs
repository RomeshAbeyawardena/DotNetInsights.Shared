using System;

namespace DotNetInsights.Shared.Contracts
{
    public interface ICatch
    {
        bool Invoke(Type exceptionType, Exception exception);
        ICatch Catch(Action<Exception> catchAction, params Type[] exceptions);
        ITry Instance { get; }
    }

    public interface ICatch<TOut> : ICatch
    {
        new ITry<TOut> Instance { get; }
    }

    public interface ICatch<TIn, TOut> : ICatch<TOut>
    {
        new ITry<TIn, TOut> Instance { get; }
    }
}