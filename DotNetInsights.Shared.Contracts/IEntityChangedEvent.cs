using DotNetInsights.Shared.Domains.Enumerations;

namespace DotNetInsights.Shared.Contracts
{
    public interface IEntityChangedEvent<TEntity> : IEvent<TEntity>
        where TEntity : class
    {
        EntityEventType EventType { get; }
    }
}
