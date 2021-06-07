using System;
using System.Linq.Expressions;

namespace Vetuviem.Core
{
    public static class ExpressionHelpers
    {
        public static Expression<Func<TView, TResult>> GetControlPropertyExpressionFromViewExpression<TView, TProp, TResult>(
            Expression<Func<TView, TProp>> viewPropExpression,
            string propertyName)
        {
            var member = typeof(TProp).GetProperty(propertyName);
            var memberAccess = Expression.MakeMemberAccess(
                viewPropExpression.Body,
                member);

            var propertyExpression = Expression.Lambda<Func<TView, TResult>>(
                    memberAccess,
                    viewPropExpression.Parameters);

            return propertyExpression;

        }
    }
}
