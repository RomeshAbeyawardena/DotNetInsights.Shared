using System;
using System.Collections.Generic;
using System.Linq;

namespace DotNetInsights.Shared.Library.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> SkipFrom<T>(this IEnumerable<T> items, T item)
        {
            var itemArray = items.ToArray();
            var foundIndex  = Array.IndexOf(itemArray, item);

            if(foundIndex < 0)
                return items;

            return items.Skip(foundIndex + 1);
        }

        public static IEnumerable<T> SkipFrom<T>(this IEnumerable<T> items, Func<T, bool> matchItem)
        {
            var itemArray = items.ToArray();
            var foundIndex  = GetIndex(itemArray, matchItem);

            if(foundIndex < 0)
                return items;

            return items.Skip(foundIndex + 1);
        }

        public static int GetIndex<T>(this IEnumerable<T> items, Func<T, bool> matchItem)
        {
            bool match(T obj)
            {
                return matchItem(obj);
            }

            return Array.FindIndex(items.ToArray(), match);
        }
    }
}
