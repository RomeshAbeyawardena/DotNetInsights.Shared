using DotNetInsights.Shared.Contracts;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace DotNetInsights.Shared.Services
{
    public class DefaultTry : ITry
    {
        private ConcurrentQueue<Action> _tryQueue;
        private readonly ICatch _catch; 

        public ICatch Catch(Action<Exception> catchAction, params Type[] exceptions)
        {
            return _catch.Catch(catchAction, exceptions);
        }

        public void Invoke()
        {
            OnInvoke(_tryQueue, queueItem => queueItem());
        }

        public ITry ThenTry(Action tryAction)
        {
            _tryQueue.Enqueue(tryAction);
            return this;
        }

        protected void OnInvoke<T>(ConcurrentQueue<T> queue, Action<T> queueInvoker)
        {
            while(queue.TryDequeue(out var tryItem))
            {
                try
                {
                    queueInvoker(tryItem);
                }
                catch(Exception ex)
                {
                    _catch.Invoke(ex.GetType());
                }
            }
        }

        protected DefaultTry(Action tryAction)
        {
            _tryQueue = new ConcurrentQueue<Action>();
            _catch = new DefaultCatch(this);
            ThenTry(tryAction);
        }

        public static ITry Try(Action tryAction)
        {
            return new DefaultTry(tryAction);
        }
    }
    
    public class DefaultTry<TOut> : DefaultTry, ITry<TOut>
    {
        private ConcurrentQueue<Func<TOut>> _tryQueue;
        private readonly ICatch<TOut> _catch; 

        protected TOut OnInvoke(ConcurrentQueue<Func<TOut>> queue, Func<Func<TOut>, TOut> queueInvoker)
        {
            
            while(queue.TryDequeue(out var tryItem))
            {
                try
                {
                    return queueInvoker(tryItem);
                }
                catch(Exception ex)
                {
                    _catch.Invoke(ex.GetType());
                }
            }

            return default;
        }

        public ICatch<TOut> Catch(Func<Exception, TOut> catchAction, params Type[] exceptions)
        {
            _catch.Catch(catchAction, exceptions);
            return _catch;
        }

        public new TOut Invoke()
        {
            return OnInvoke(_tryQueue, queueItem => queueItem());
        }

        public ITry<TOut> ThenTry(Func<TOut> tryAction)
        {
            _tryQueue.Enqueue(tryAction);
            return this;
        }

        protected DefaultTry(Func<TOut> tryAction)
            : base(null)
        {
            _tryQueue = new ConcurrentQueue<Func<TOut>>();
            _catch = new DefaultCatch<TOut>(this);
            ThenTry(tryAction);
        }

        public static ITry<TOut> Try(Func<TOut> tryAction)
        {
            return new DefaultTry<TOut>(tryAction);
        }

    }

    public class DefaultTry<TIn, TOut> : DefaultTry<TOut>, ITry<TIn, TOut>
    {
        private ConcurrentQueue<Func<TIn, TOut>> _tryQueue;
        private readonly ICatch<TOut> _catch; 

        public ICatch<TOut> Catch(Func<Exception, TOut> catchAction, params Type[] exceptions)
        {
            _catch.Catch(catchAction, exceptions);
            return _catch;
        }

        protected TOut OnInvoke(ConcurrentQueue<Func<TIn, TOut>> queue, Func<Func<TIn, TOut>, TOut> queueInvoker)
        {
            while(queue.TryDequeue(out var tryItem))
            {
                try
                {
                    return queueInvoker(tryItem);
                }
                catch(Exception ex)
                {
                    _catch.Invoke(ex.GetType());
                }
            }

            return default;
        }

        public TOut Invoke(TIn value)
        {
            return OnInvoke(_tryQueue, item => item(value));
        }

        public ITry<TIn, TOut> ThenTry(Func<TIn, TOut> tryAction)
        {
            _tryQueue.Enqueue(tryAction);
            return this;
        }

        protected DefaultTry(Func<TIn, TOut> tryAction)
            : base(null)
        {
            _tryQueue = new ConcurrentQueue<Func<TIn, TOut>>();
            _catch = new DefaultCatch<TOut>(this);
            ThenTry(tryAction);
        }

        public static ITry<TIn, TOut> Try(Func<TIn, TOut> tryAction)
        {
            return new DefaultTry<TIn, TOut>(tryAction);
        }
    }
}