using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reactive.Disposables;
using System.Reflection;
using System.Text;
using ReactiveUI;

namespace Vetuviem.Core
{
    /// <summary>
    /// Represents a direct assignment a View Property that supports One or Two way binding. This is used when you want to set a property on the View directly without binding it to a property on the ViewModel.
    /// Use cases for this are when a property on the ViewModel is a POCO and can't change. This is also used when you want to set a property on the View that doesn't have a corresponding property on the ViewModel.
    /// </summary>
    /// <typeparam name="TVetuviemTargetViewModel">The type for the target ViewModel that Vetuviem will bind to.</typeparam>
    /// <typeparam name="TViewProp">The type for the View.</typeparam>
    public sealed class DirectAssignment<TVetuviemTargetViewModel, TViewProp> : IOneOrTwoWayBind<TVetuviemTargetViewModel, TViewProp>
        where TVetuviemTargetViewModel : class
    {
        private readonly Func<TVetuviemTargetViewModel, TViewProp?> _assignmentFunc;

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectAssignment{TVetuviemTargetViewModel, TViewProp}"/> class.
        /// </summary>
        /// <param name="assignmentFunc">Func for the View Model binding.</param>
        public DirectAssignment(Func<TVetuviemTargetViewModel, TViewProp?> assignmentFunc)
        {
            _assignmentFunc = assignmentFunc ?? throw new ArgumentNullException(nameof(assignmentFunc));
        }

        /// <inheritdoc/>
        public void ApplyBinding<TView>(
            Action<IDisposable> d,
            TView view,
            TVetuviemTargetViewModel viewModel,
            Expression<Func<TView, TViewProp>> viewBinding)
            where TView : class, IViewFor<TVetuviemTargetViewModel>
        {
            if (d == null)
            {
                throw new ArgumentNullException(nameof(d));
            }

            DoAssignment(
                view,
                viewModel,
                viewBinding);
        }

        /// <inheritdoc/>
        public void ApplyBinding<TView>(
            CompositeDisposable compositeDisposable,
            TView view,
            TVetuviemTargetViewModel viewModel,
            Expression<Func<TView, TViewProp>> viewBinding)
            where TView : class, IViewFor<TVetuviemTargetViewModel>
        {
            if (compositeDisposable == null)
            {
                throw new ArgumentNullException(nameof(compositeDisposable));
            }

            DoAssignment(
                view,
                viewModel,
                viewBinding);
        }

        private void DoAssignment<TView>(TView view, TVetuviemTargetViewModel viewModel, Expression<Func<TView, TViewProp>> viewBinding) where TView : class, IViewFor<TVetuviemTargetViewModel>
        {
            // Extract the property from the expression
            if (viewBinding.Body is not MemberExpression memberExpression ||
                memberExpression.Member is not PropertyInfo propertyInfo)
            {
                throw new ArgumentException("viewBinding must be a simple property expression", nameof(viewBinding));
            }

            propertyInfo.SetValue(
                view,
                _assignmentFunc(viewModel));
        }
    }
}
