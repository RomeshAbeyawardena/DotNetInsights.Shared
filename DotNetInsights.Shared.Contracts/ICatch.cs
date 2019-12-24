using System;

namespace DotNetInsights.Shared.Contracts
{
    public interface ICatch
    {
        void Invoke(Type exceptionType);
        ICatch Catch(Action<Exception> catchAction, params Type[] exceptions);
        ITry Instance { get; }
    }

    public interface ICatch<TOut> : ICatch
    {
        ICatch<TOut> Catch(Func<Exception, TOut> catchReturnAction, params Type[] exceptions);
        new ITry<TOut> Instance { get; }
    }

    public interface ICatch<TIn, TOut> : ICatch<TOut>
    {
        ICatch<TIn, TOut> Catch(Func<Exception, TIn, TOut> catchReturnAction, params Type[] exceptions);
        new ITry<TIn, TOut> Instance { get; }
    }
}