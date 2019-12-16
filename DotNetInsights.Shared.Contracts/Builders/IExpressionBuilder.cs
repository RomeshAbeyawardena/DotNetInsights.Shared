using System;
using System.Linq.Expressions;
using DotNetInsights.Shared.Domains.Enumerations;

namespace DotNetInsights.Shared.Contracts.Builders
{
    public interface IExpressionBuilder
    {
        IExpressionBuilder And(string name, ExpressionComparer? expressionComparer = null, object value = null);
        IExpressionBuilder Or(string name, ExpressionComparer? expressionComparer = null, object value = null);
        IExpressionBuilder Not(string name, ExpressionComparer? expressionComparer = null, object value = null);
        Expression<Func<TEntity, bool>> ToExpression<TEntity>();
    }
}
