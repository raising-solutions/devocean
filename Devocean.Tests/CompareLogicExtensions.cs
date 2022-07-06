using System.Linq.Expressions;
using System.Reflection;
using KellermanSoftware.CompareNetObjects;

namespace Devocean.Tests;

public static class CompareLogicExtensions {

    public static CompareLogic IgnoreProperty<TType>(this CompareLogic compareLogic, Expression<Func<TType, Object>> property)
    {
        LambdaExpression lambda = property;
        var memberExpression = lambda.Body is UnaryExpression expression
            ? (MemberExpression)expression.Operand
            : (MemberExpression)lambda.Body;
        
        compareLogic.Config.MembersToIgnore ??= new List<string>();
        compareLogic.Config.MembersToIgnore.Add(memberExpression.Member.Name);
        
        return compareLogic;
    }
}