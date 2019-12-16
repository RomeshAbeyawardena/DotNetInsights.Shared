using DotNetInsights.Shared.Contracts.Providers;
using System;
using System.Collections;
using System.Collections.Generic;

namespace DotNetInsights.Shared.Services.Builders
{
    public static class ListBuilder
    {
        public static IListBuilder<T> Create<T>()
        {
            return new ListBuilder<T>();
        }
    }

    public sealed class ListBuilder<T> : IListBuilder<T>
    {
        public T this[int index] => list[index];

        public int Count => list.Count;

        public IListBuilder<T> Add(T item)
        {
            list.Add(item);
            return this;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return list.GetEnumerator();
        }

        public IListBuilder<T> AddRange(IEnumerable<T> values)
        {

            foreach(var value in values)
                Add(value);

            return this;
        }

        public IListBuilder<T> Insert(T value, int index)
        {
            throw new NotImplementedException();
        }

        public ListBuilder()
        {
            list = new List<T>();
        }

        private readonly IList<T> list;
    }
}
