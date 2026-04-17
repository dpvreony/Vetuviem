using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using ReactiveUI.WPF.SampleApp.ViewModels;
using ReactiveUI.WPF.SampleApp.Views;
using Vetuviem.Core;
using VetuviemGenerated.Wpf.ViewToViewModelBindings.System.Windows.Controls;
using VetuviemGenerated.Wpf.ViewToViewModelBindings.System.Windows.Controls.Primitives;

namespace ReactiveUI.WPF.SampleApp.ViewBindingModels
{
    internal static class ViewPropertyBindingExtensions
    {
        public static ButtonControlBindingModel<TView, TViewModel> GetDefaultBindingControlModel<TView, TViewModel>(
            this Expression<Func<TView, Button>> expression,
            ICommandBinding<TViewModel>? bindCommand)
            where TView : class, IViewFor<TViewModel>
            where TViewModel : class, IReactiveObject
        {
            return new ButtonControlBindingModel<TView, TViewModel>(expression)
            {
                BindCommand = bindCommand
            };
        }

        public static ButtonBaseControlBindingModel<TView, TViewModel> GetDefaultBindingControlModel<TView, TViewModel>(
            this Expression<Func<TView, ButtonBase>> expression,
            ICommandBinding<TViewModel>? bindCommand)
            where TView : class, IViewFor<TViewModel>
            where TViewModel : class, IReactiveObject
        {
            return new ButtonBaseControlBindingModel<TView, TViewModel>(expression)
            {
                BindCommand = bindCommand
            };
        }

        public static LabelControlBindingModel<QuestionnaireView, QuestionnaireViewModel> GetStandardLengthRemainingLabelViewBindingModel(
            this Expression<Func<QuestionnaireView, Label>> controlExpression,
            Expression<Func<QuestionnaireViewModel, object?>> viewModelObjectExpression,
            Expression<Func<QuestionnaireViewModel, int>> viewModelNumberExpression)
        {
            return new(controlExpression)
            {
                // TODO: explore the ability to pass in an object and not apply a vm convertor. this is doing boxing we can probably avoid
                Content = new OneWayBindingOnOneOrTwoWayBind<QuestionnaireViewModel, object>(viewModelObjectExpression, o => o?.ToString() ?? string.Empty),
                Foreground = new OneWayBindingWithConversionOnOneOrTwoWayBind<QuestionnaireViewModel, Brush, int>(viewModelNumberExpression, lengthRemaining => GetBrushForLengthRemaining(lengthRemaining))
            };
        }

        public static TextBoxControlBindingModel<QuestionnaireView, QuestionnaireViewModel> GetStandardTextBoxViewModel(
            this Expression<Func<QuestionnaireView, TextBox>> controlExpression,
            Expression<Func<QuestionnaireViewModel, string?>> viewModelTextExpression)
        {
            return new(controlExpression)
            {
                // max length is used in this scenario because you may have a flexible data capture form
                // in this sample it's fixed, but also used for calculating the limit on the label
                // we actually set the control max length slightly longer than the desired max length
                // this is to do with capturing a bad paste from the user. where it would cap at the max length of the
                // control, and they wouldn't be aware they passed the limit and lost data.
                MaxLength = new OneWayBindingOnOneOrTwoWayBind<QuestionnaireViewModel, int>(vm => vm.MaxLength, x => x + 10),
                Text = new TwoWayBinding<QuestionnaireViewModel, string>(viewModelTextExpression),
            };
        }

        private static SolidColorBrush GetBrushForLengthRemaining(int lengthRemaining)
        {
            return lengthRemaining switch
            {
                < 0 => Brushes.Red,
                < 10 => Brushes.Orange,
                < 20 => Brushes.Gold,
                _ => Brushes.Black
            };
        }
    }
}
