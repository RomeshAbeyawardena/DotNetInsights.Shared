using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DotNetInsights.Shared.Library.Extensions
{
    public static class ObjectCollectionExtensions
    {
        public static IEnumerable<T> Remove<T>(this IEnumerable<T> items, T entry)
        {
            var itemList = new List<T>(items);
            itemList.Remove(entry);
            return itemList.ToArray();
        }

        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> iterator)
        {
            foreach (var item in collection)
            {
                iterator(item);
            }
        }

        public static void ForEach<T>(this IEnumerable<T> collection, Action<T, int> iterator)
        {
            var index = 0;
            foreach (var item in collection)
            {
                iterator(item, index++);
            }
        }

        public static async Task ForEach<T>(this IEnumerable<T> collection, Func<T, Task> asyncIterator)
        {
            foreach (var item in collection)
            {
                await asyncIterator(item).ConfigureAwait(false);
            }
        }

        public static async Task ForEach<T>(this IEnumerable<T> collection, Func<T, int, Task> asyncIterator)
        {
            var index = 0;
            foreach (var item in collection)
            {
                await asyncIterator(item, index++).ConfigureAwait(false);
            }
        }
    }
}
