using DotNetInsights.Shared.Contracts;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetInsights.Shared.Services
{
    public static class ExceptionHandler
    {
        public static ITry Try(this Action @try)
        {
            return DefaultTry.Try(@try);
        }

        public static ITry<Task> TryAsync(this Func<Task> @try)
        {
            return Try(@try);
        }

        public static ITry<Task<TOut>> TryAsync<TOut>(this Func<Task<TOut>> @try)
        {
            return Try(@try);
        }

        public static ITry<TIn, Task<TOut>> TryAsync<TIn, TOut>(this Func<TIn, Task<TOut>> @try)
        {
            return DefaultTry<TIn, Task<TOut>>.Try(@try);
        }

        public static ITry<TOut> Try<TOut>(this Func<TOut> @try)
        {
            return DefaultTry<TOut>.Try(@try);
        }

        public static ITry<TIn, TOut> Try<TIn, TOut>(this Func<TIn, TOut> @try)
        {
            return DefaultTry<TIn, TOut>.Try(@try);
        }
    }
}
