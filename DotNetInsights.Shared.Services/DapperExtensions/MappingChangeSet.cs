using DotNetInsights.Shared.Contracts.DapperExtensions;
using DotNetInsights.Shared.Domains.Enumerations;
using System.Collections.Generic;
using System.Reflection;

namespace DotNetInsights.Shared.Services.DapperExtensions
{
    public class MappingChangeSet<T> : IChangeSet<T>
    {
        public MappingChangeSet(IEnumerable<PropertyInfo> properties,T currentEntity, T newEntity, IQueryParameters<T> queryParameters)
        {

            CurrentEntity = currentEntity;
            NewEntity = newEntity;
            QueryParameters = queryParameters;
            ChangeSetDictionary = new Dictionary<PropertyInfo, ChangeState>();
            foreach (var property in properties)
            {
                var newValue = property.GetValue(newEntity);
                var originalValue = property.GetValue(currentEntity);

                var changeState = ChangeState.Unchanged;

                if (!originalValue.Equals(newValue))
                {
                    ChangeState = ChangeState.Modified;
                    changeState = ChangeState.Modified;
                }

                ChangeSetDictionary.Add(property, changeState);
            }
        }

        public T CurrentEntity { get;}
        public T NewEntity { get; }
        public IDictionary<PropertyInfo, ChangeState> ChangeSetDictionary { get; private set; }
        public ChangeState ChangeState { get; set; }
        public IQueryParameters<T> QueryParameters { get; }
    }
}