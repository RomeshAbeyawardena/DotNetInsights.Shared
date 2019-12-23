using System;

namespace DotNetInsights.Shared.Contracts
{
    public interface ICatch
    {
        void Invoke(Exception exception);
        ICatch Catch(Action catchAction, params Exception[] exceptions);
    }

    public interface ICatch<TOut>
    {
        ICatch<TOut> Catch(Func<TOut> catchReturnAction, params Exception[] exceptions);
    }
}