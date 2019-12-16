using DotNetInsights.Shared.Contracts;
using DotNetInsights.Shared.Domains;
using System.Collections.Generic;
using DotNetInsights.Shared.Domains.Enumerations;

namespace DotNetInsights.Shared.Services
{
    public static class DefaultEntityChangedEvent
    {
        public static IEntityChangedEvent<T> Create<T>(T result, IEnumerable<T> results = null, EntityEventType entityEventType = EntityEventType.None)
            where T: class
        {
            return new DefaultEntityChangedEvent<T>(result, results, entityEventType);
        }
    }

    public class DefaultEntityChangedEvent<T> : DefaultEvent<T>, IEntityChangedEvent<T>
        where T: class
    {
        public DefaultEntityChangedEvent(T result, IEnumerable<T> results = null, EntityEventType entityEventType = EntityEventType.None)
            : base(result, results)
        {
            EventType = entityEventType;
        }

        public EntityEventType EventType { get; }
    }
}
