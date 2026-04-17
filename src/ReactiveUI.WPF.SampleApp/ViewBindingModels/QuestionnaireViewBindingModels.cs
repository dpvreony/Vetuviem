// Copyright (c) 2022 DPVreony and Contributors. All rights reserved.
// DPVreony and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ReactiveUI.WPF.SampleApp.ViewModels;
using ReactiveUI.WPF.SampleApp.Views;
using VetuviemGenerated.Wpf.ViewToViewModelBindings.System.Windows.Controls;
using Vetuviem.Core;

namespace ReactiveUI.WPF.SampleApp.ViewBindingModels
{
    /// <summary>
    /// View Binding Model for the Questionnaire View Model.
    /// </summary>
    public sealed class QuestionnaireViewBindingModels : AbstractEnableViewToViewModelBindings<QuestionnaireView, QuestionnaireViewModel>
    {
        /// <inheritdoc />
        protected override IEnumerable<IControlBindingModel<QuestionnaireView, QuestionnaireViewModel>> GetBindings()
        {
            // launch interaction
            yield return this.ForViewProperty(vw => vw.LaunchInteraction)
                .GetDefaultBindingControlModel(new CommandBinding<QuestionnaireViewModel>(vm => vm.LaunchInteraction));

            // Forename
            yield return this.ForViewProperty(vw => vw.Forename)
                .GetStandardTextBoxViewModel(vm => vm.Forename);

            yield return this.ForViewProperty(vw => vw.ForenameLengthRemaining)
                .GetStandardLengthRemainingLabelViewBindingModel(
                    vm => vm.ForenameLengthRemaining,
                    vm => vm.ForenameLengthRemaining);

            // Surname
            yield return this.ForViewProperty(vw => vw.Surname)
                .GetStandardTextBoxViewModel(vm => vm.Surname);

            yield return this.ForViewProperty(vw => vw.SurnameLengthRemaining)
                .GetStandardLengthRemainingLabelViewBindingModel(
                    vm => vm.SurnameLengthRemaining,
                    vm => vm.SurnameLengthRemaining);

            // answer one
            yield return this.ForViewProperty(vw => vw.AnswerOne)
                .GetStandardTextBoxViewModel(vm => vm.AnswerOne);

            yield return this.ForViewProperty(vw => vw.AnswerOneLengthRemaining)
                .GetStandardLengthRemainingLabelViewBindingModel(
                    vm => vm.AnswerOneLengthRemaining,
                    vm => vm.AnswerOneLengthRemaining);

            // answer two
            yield return this.ForViewProperty(vw => vw.AnswerTwo)
                .GetStandardTextBoxViewModel(vm => vm.AnswerTwo);

            yield return this.ForViewProperty(vw => vw.AnswerTwoLengthRemaining)
                .GetStandardLengthRemainingLabelViewBindingModel(
                    vm => vm.AnswerTwoLengthRemaining,
                    vm => vm.AnswerTwoLengthRemaining);

            // answer three
            yield return this.ForViewProperty(vw => vw.AnswerThree)
                .GetStandardTextBoxViewModel(vm => vm.AnswerThree);
            yield return this.ForViewProperty(vw => vw.AnswerThreeLengthRemaining)
                .GetStandardLengthRemainingLabelViewBindingModel(
                    vm => vm.AnswerThreeLengthRemaining,
                    vm => vm.AnswerThreeLengthRemaining);

            // answer four
            yield return this.ForViewProperty(vw => vw.AnswerFour)
                .GetStandardTextBoxViewModel(vm => vm.AnswerFour);

            yield return this.ForViewProperty(vw => vw.AnswerFourLengthRemaining)
                .GetStandardLengthRemainingLabelViewBindingModel(
                    vm => vm.AnswerFourLengthRemaining,
                    vm => vm.AnswerFourLengthRemaining);

            // answer five
            yield return this.ForViewProperty(vw => vw.AnswerFive)
                .GetStandardTextBoxViewModel(vm => vm.AnswerFive);

            yield return this.ForViewProperty(vw => vw.AnswerFiveLengthRemaining)
                .GetStandardLengthRemainingLabelViewBindingModel(
                    vm => vm.AnswerFiveLengthRemaining,
                    vm => vm.AnswerFiveLengthRemaining);
        }
    }
}
