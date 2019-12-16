using System;
using System.Threading;
using System.Threading.Tasks;

namespace DotNetInsights.Shared.Library.Extensions
{
    public static class ObjectExtensions
    {
        public static bool IsNullOrDefault(this object o)
        {
            return (o == null
                    || (o is uint oUInt && oUInt == default)
                    || (o is int oInt && oInt == default)
                    || (o is float oFloat && oFloat == default)
                    || (o is double oDouble && oDouble == default)
                    || (o is decimal odecimal && odecimal == default)
                    || (o is short oShort && oShort == default)
                    || (o is long oLong && oLong == default)
                    || (o is DateTime oDateTime && oDateTime == default)
                    || (o is DateTimeOffset oDateTimeOffset && oDateTimeOffset == default)
                    || (o is string oString && string.IsNullOrEmpty(oString))
                   );
        }

        public static object ValueOrDefault(this object value, object @default)
        {
            if (IsNullOrDefault(value)) 
                return @default;
            
            return value;
        }

        public static bool TryParse(this string value, out int result)
        {
            return int.TryParse(value, out result);
        }

        public static bool TryParse(this string value, out bool result)
        {
            return bool.TryParse(value, out result);
        }

        public static bool TryParse(this string value, out decimal result)
        {
            return decimal.TryParse(value, out result);
        }

        public static bool TryParse(this string value, out double result)
        {
            return double.TryParse(value, out result);
        }

        public static bool TryParse(this string value, out float result)
        {
            return float.TryParse(value, out result);
        }

        public static bool TryParse(this string value, out DateTime result)
        {
            return DateTime.TryParse(value, out result);
        }

        public static bool TryParse(this string value, out Guid result)
        {
            return Guid.TryParse(value, out result);
        }

        public static bool TryParse(this string value, out DateTimeOffset result)
        {
            return DateTimeOffset.TryParse(value, out result);
        }

        public static bool TryParse<T>(this object value, out T? result)
            where T: struct
        {
            result = default;
            
            if(!(value is T tResult))
                return false;
            
            result = tResult;
            
            return true;
        }

        public static bool TryParse<T>(this object value, out T result)
            where T: class
        {
            result = default;
            
            if(!(value is T tResult))
                return false;
            
            result = tResult;
            
            return true;
        }

        public static void AsLock(this object value, Action onLock)
        {
            lock (value)
            {
                onLock();
            }
        }

        public static T AsLock<T>(this object value, Func<T> onLock)
        {
            lock (value)
            {
                return onLock();
            }
        }

        public static async Task AsLockAsync(this SemaphoreSlim semaphoreSlim , Func<Task> onLock)
        {
            await semaphoreSlim.WaitAsync().ConfigureAwait(false);
            try
            {
                await onLock().ConfigureAwait(false);
            }
            finally
            {
                semaphoreSlim.Release();
            }
            
        }

        public static async Task<T> AsLockAsync<T>(this SemaphoreSlim semaphoreSlim , Func<Task<T>> onLock)
        {
            await semaphoreSlim.WaitAsync().ConfigureAwait(false);
            try{
                return await onLock().ConfigureAwait(false);
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }
    }
}
