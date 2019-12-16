using System;
using System.Threading.Tasks;

namespace DotNetInsights.Shared.Library.Extensions
{
    public static class DisposableExtensions
    {
        public static void Use<T>(this T disposable, Action<T> onUse) 
            where T : IDisposable
        {
            using (disposable)
            {
                onUse(disposable);
            }
        }

        public static TOut Use<T, TOut>(this T disposable, Func<T, TOut> onUse) 
            where T : IDisposable
        {
            using (disposable)
            {
                return onUse(disposable);
            }
        }

        public static async Task UseAsync<T>(this T disposable, Func<T, Task> onUse)
            where T : IDisposable
        {
            using (disposable)
            {
                await onUse(disposable).ConfigureAwait(false);
            }
        }

        public static async Task<TOut> UseAsync<T, TOut>(this T disposable, Func<T, Task<TOut>> onUse) 
            where T : IDisposable
        {
            return await Use(disposable, onUse).ConfigureAwait(false);
        }
    }
}
