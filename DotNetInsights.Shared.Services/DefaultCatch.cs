using DotNetInsights.Shared.Contracts;
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace DotNetInsights.Shared.Services
{
    public class DefaultCatch : ICatch
    {
        private ISwitch<Exception, Action> _catchSwitch;
        public ICatch Catch(Action catchAction, params Exception[] exceptions)
        {
            _catchSwitch.CaseWhen(exceptions.FirstOrDefault(), catchAction, exceptions);
            return this;
        }

        public virtual void Invoke(Exception exception)
        {
            _catchSwitch.Case(exception);
        }
    }
    public sealed class DefaultCatch<TOut> : DefaultCatch, ICatch<TOut>
    {
        private ISwitch<Exception, Func<TOut>> _catchSwitch;
        public ICatch<TOut> Catch(Func<TOut> catchReturnAction, params Exception[] exceptions)
        {
            _catchSwitch.CaseWhen(exceptions.FirstOrDefault(), catchReturnAction, exceptions);
            return this;
        }

        public DefaultCatch()
        {
            _catchSwitch = DefaultSwitch.Create<Exception, Func<TOut>>();
        }
    }
    public sealed class DefaultTry<TOut, TIn> : DefaultTry<TOut>, ITry<TOut, TIn>
    {
        private readonly ConcurrentQueue<Func<TOut, TIn>> _queue;
        
        private DefaultTry(Func<TOut, TIn> tryAction)
            : base(null)
        {
            _queue = new ConcurrentQueue<Func<TOut, TIn>>();
            ThenTry(tryAction);
        }

        public ITry<TOut, TIn> ThenTry(Func<TOut, TIn> tryAction)
        {
            _queue.Enqueue(tryAction);
            return this;
        }


    }
}