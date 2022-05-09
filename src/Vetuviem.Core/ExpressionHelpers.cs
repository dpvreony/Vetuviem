// Copyright (c) 2022 DPVreony and Contributors. All rights reserved.
// DPVreony and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Linq.Expressions;

namespace Vetuviem.Core
{
    /// <summary>
    /// Helper for manipulating the view expressions.
    /// </summary>
    public static class ExpressionHelpers
    {
        /// <summary>
        /// Takes a view property expression and rewrites it to target the control property.
        /// </summary>
        /// <typeparam name="TView">The type for the view.</typeparam>
        /// <typeparam name="TProp">The type for the property, which will be a control in the different UI frameworks.</typeparam>
        /// <typeparam name="TResult">The type for the bound result.</typeparam>
        /// <param name="viewPropExpression">Expression representing the view property.</param>
        /// <param name="propertyName">The name of the property to target.</param>
        /// <returns>Expression for binding to a control property.</returns>
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
        /// Converts an expression for a property on a control to be an expression to a base class.
        /// </summary>
        /// <typeparam name="TView">The type for view the control is on.</typeparam>
        /// <typeparam name="TProp">The type for the property.</typeparam>
        /// <typeparam name="TResult">The base type for the property.</typeparam>
        /// <param name="viewPropExpression">Expression representing the derived control.</param>
        /// <returns>Expression for targeting the member on the base class.</returns>
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
