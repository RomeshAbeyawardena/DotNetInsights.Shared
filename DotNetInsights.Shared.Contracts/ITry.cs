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
        ICatch<TOut> Catch(Func<Exception, TOut> catchAction, params Type[] exceptions);
    }

    public interface ITry<TIn, TOut> : ITry
    {
        TOut Invoke(TIn value);
        ITry<TIn, TOut> ThenTry(Func<TIn, TOut> tryAction);
        ICatch<TIn, TOut> Catch(Func<Exception, TOut> catchAction, params Type[] exceptions);
    }
}