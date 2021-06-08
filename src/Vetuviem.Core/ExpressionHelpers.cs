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

        /// <summary>
        /// Converts an expression for a property on a control to be an expression to a base class
        /// </summary>
        /// <typeparam name="TView">The type for view the control is on.</typeparam>
        /// <typeparam name="TProp">The type for the property.</typeparam>
        /// <typeparam name="TResult">The base type for the property.</typeparam>
        /// <param name="viewPropExpression">Expression representing the derived control.</param>
        /// <returns></returns>
        public static Expression<Func<TView, TResult>> ConvertControlExpressionToBaseClassExpression<TView, TProp, TResult>(Expression<Func<TView, TProp>> viewPropExpression)
            where TProp : class, TResult
            where TResult : class
        {
            var propertyExpression = Expression.Lambda<Func<TView, TResult>>(
                viewPropExpression.Body,
                viewPropExpression.Parameters);

            return propertyExpression;

        }
    }
}
