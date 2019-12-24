using DotNetInsights.Shared.Contracts;
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace DotNetInsights.Shared.Services
{
    public class DefaultCatch : ICatch
    {
        private readonly ISwitch<Type, Action<Exception>> _catchSwitch;
        public ICatch Catch(Action<Exception> catchAction, params Type[] exceptions)
        {
            _catchSwitch.CaseWhen(exceptions.FirstOrDefault(), catchAction, exceptions);
            return this;
        }

        public virtual void Invoke(Type exceptionType)
        {
            _catchSwitch.Case(exceptionType);
        }

        public ITry Instance { get; private set; }

        public DefaultCatch(ITry tryInstance)
        {
            Instance = tryInstance;
        }
    }
    public class DefaultCatch<TOut> : DefaultCatch, ICatch<TOut>
    {
        private ISwitch<Type, Func<Exception, TOut>> _catchSwitch;
        public ICatch<TOut> Catch(Func<Exception, TOut> catchReturnAction, params Type[] exceptions)
        {
            _catchSwitch.CaseWhen(exceptions.FirstOrDefault(), catchReturnAction, exceptions);
            return this;
        }

        public new ITry<TOut> Instance { get; private set; }

        public DefaultCatch(ITry<TOut> tryInstance)
            : base(tryInstance)
        {
            Instance = tryInstance;
            _catchSwitch = DefaultSwitch.Create<Type, Func<Exception, TOut>>();
        }

        protected DefaultCatch(ITry tryInstance)
            : base(tryInstance)
        {
            _catchSwitch = DefaultSwitch.Create<Type, Func<Exception, TOut>>();
        }
    }

    public sealed class DefaultCatch<TIn, TOut> : DefaultCatch<TOut>, ICatch<TIn, TOut>
    {
        private ISwitch<Type, Func<Exception, TIn, TOut>> _catchSwitch;
        public DefaultCatch(ITry<TIn, TOut> tryInstance)
            : base(tryInstance)
        {

        }

        public new ITry<TIn, TOut> Instance { get; }

        public ICatch<TIn, TOut> Catch(Func<Exception, TIn, TOut> catchReturnAction, params Type[] exceptions)
        {
            _catchSwitch.CaseWhen(exceptions.FirstOrDefault(), catchReturnAction, exceptions);
            return this;
        }
    }
}