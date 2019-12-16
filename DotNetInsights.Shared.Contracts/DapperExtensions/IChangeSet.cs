using DotNetInsights.Shared.Domains.Enumerations;
using System.Collections.Generic;
using System.Reflection;

namespace DotNetInsights.Shared.Contracts.DapperExtensions
{
    public interface IChangeSet<T>
    {
        T CurrentEntity { get; }
        T NewEntity { get; }
        IDictionary<PropertyInfo, ChangeState> ChangeSetDictionary { get; }
        ChangeState ChangeState { get; set; }
        IQueryParameters<T> QueryParameters { get; }
    }
}