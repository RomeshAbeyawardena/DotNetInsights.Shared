using System;
using System.Collections.Generic;

namespace DotNetInsights.Shared.Contracts
{
    public interface ITry
    {
        void Invoke();
        ITry ThenTry(Action tryAction);
        ICatch Catch(Action catchAction, params Exception[] exceptions);
    }

    public interface ITry<TOut> : ITry
    {
        new IEnumerable<TOut> Invoke();
        ITry<TOut> ThenTry(Func<TOut> tryAction);
        ICatch<TOut> Catch(Func<TOut> catchAction, params Exception[] exceptions);
    }

    public interface ITry<TOut, TIn> : ITry
    {
        TOut Invoke(TIn value);
        ITry<TOut, TIn> ThenTry(Func<TOut, TIn> tryAction);
        ICatch<TOut> Catch(Func<TOut> catchAction, params Exception[] exceptions);
    }
}