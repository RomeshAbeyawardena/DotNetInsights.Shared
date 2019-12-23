using DotNetInsights.Shared.Contracts;
using System;
using System.Collections.Concurrent;

namespace DotNetInsights.Shared.Services
{
    public class DefaultTry : ITry
    {
        private ConcurrentQueue<Action> _tryQueue;
        private readonly ICatch _catch; 

        public ICatch Catch(Action catchAction, params Exception[] exceptions)
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
                    _catch.Invoke(ex);
                }
            }
        }

        protected DefaultTry(Action tryAction)
        {
            _tryQueue = new ConcurrentQueue<Action>();
            _catch = new DefaultCatch();
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

        public ICatch<TOut> Catch(Func<TOut> catchAction, params Exception[] exceptions)
        {
            _catch.Catch(catchAction, exceptions);
            return _catch;
        }

        public TOut Invoke()
        {
            OnInvoke(_tryQueue, queueItem => queueItem());
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
            _catch = new DefaultCatch<TOut>();
            ThenTry(tryAction);
        }

        public static ITry<TOut> Try(Func<TOut> tryAction)
        {
            return new DefaultTry<TOut>(tryAction);
        }
    }

    public class DefaultTry<TOut, TIn> : DefaultTry<TOut>, ITry<TOut, TIn>
    {
        private ConcurrentQueue<Func<TOut>> _tryQueue;
        private readonly ICatch<TOut> _catch; 

        public ICatch<TOut> Catch(Func<TOut> catchAction, params Exception[] exceptions)
        {
            _catch.Catch(catchAction, exceptions);
            return _catch;
        }

        public override void Invoke(TIn value)
        {
            throw new NotImplementedException();
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
            _catch = new DefaultCatch<TOut>();
            ThenTry(tryAction);
        }

        public static ITry<TOut> Try(Func<TOut> tryAction)
        {
            return new DefaultTry<TOut>(tryAction);
        }
    }
}