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

        public virtual bool Invoke(Type exceptionType, Exception exception)
        {
            var catchcase = _catchSwitch.Case(exceptionType);

            if(catchcase == null)
                return false;

            catchcase(exception);
            return true;
        }

        public ITry Instance { get; private set; }

        public DefaultCatch(ITry tryInstance)
        {
            _catchSwitch = DefaultSwitch.Create<Type, Action<Exception>>();
            Instance = tryInstance;
        }
    }
    public class DefaultCatch<TOut> : DefaultCatch, ICatch<TOut>
    {
        
        public new ITry<TOut> Instance { get; private set; }

        public DefaultCatch(ITry<TOut> tryInstance)
            : base(tryInstance)
        {
            Instance = tryInstance;
            
        }

        protected DefaultCatch(ITry tryInstance)
            : base(tryInstance)
        {
            
        }
    }

    public sealed class DefaultCatch<TIn, TOut> : DefaultCatch<TOut>, ICatch<TIn, TOut>
    {
        public new ITry<TIn, TOut> Instance { get; }
        public DefaultCatch(ITry<TOut> tryInstance)
            : base (tryInstance)
        {

        }
        
    }
}