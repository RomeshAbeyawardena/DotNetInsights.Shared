using System;
using System.Collections.Generic;

namespace DotNetInsights.Shared.Contracts
{
    public interface ITry
    {
        void Invoke();
        ITry ThenTry(Action tryAction);
        ICatch Catch(Action<Exception> catchAction, params Type[] exceptions);
    }

    public interface ITry<TOut> : ITry
    {
        new TOut Invoke();
        ITry<TOut> ThenTry(Func<TOut> tryAction);
        new ICatch<TOut> Catch(Action<Exception> catchAction, params Type[] exceptions);
    }

    public interface ITry<TIn, TOut> : ITry
    {
        TOut Invoke(TIn value);
        ITry<TIn, TOut> ThenTry(Func<TIn, TOut> tryAction);
        new ICatch<TIn, TOut> Catch(Action<Exception> catchAction, params Type[] exceptions);
    }
}