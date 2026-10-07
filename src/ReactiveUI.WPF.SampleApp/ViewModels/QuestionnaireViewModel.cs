// Copyright (c) 2022 DPVreony and Contributors. All rights reserved.
// DPVreony and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ReactiveUI.Primitives;

namespace ReactiveUI.WPF.SampleApp.ViewModels
{
    /// <summary>
    /// A View Model for tracking a questionnaire.
    /// </summary>
    public sealed class QuestionnaireViewModel : ReactiveObject
    {
        // this is here to remove the warning to make a property static if it's direct on the property instead of in a variable.
        private readonly int _maxLength = 50;
        private readonly ObservableAsPropertyHelper<int> _forenameLengthRemaining;
        private readonly ObservableAsPropertyHelper<int> _surnameLengthRemaining;
        private readonly ObservableAsPropertyHelper<int> _answerOneLengthRemaining;
        private readonly ObservableAsPropertyHelper<int> _answerTwoLengthRemaining;
        private readonly ObservableAsPropertyHelper<int> _answerThreeLengthRemaining;
        private readonly ObservableAsPropertyHelper<int> _answerFourLengthRemaining;
        private readonly ObservableAsPropertyHelper<int> _answerFiveLengthRemaining;

        /// <summary>
        /// Initializes a new instance of the <see cref="QuestionnaireViewModel"/> class.
        /// </summary>
        public QuestionnaireViewModel()
        {
            _forenameLengthRemaining = this.WhenChanged(vm => vm.Forename)
                .Select(forename => _maxLength - (forename?.Length ?? 0))
                .ToProperty(
                    this,
                    vw => vw.ForenameLengthRemaining);

            _surnameLengthRemaining = this.WhenChanged(vm => vm.Surname)
                .Select(surname => _maxLength - (surname?.Length ?? 0))
                .ToProperty(
                    this,
                    vw => vw.SurnameLengthRemaining);

            _answerOneLengthRemaining = this.WhenChanged(vm => vm.AnswerOne)
                .Select(answerOne => _maxLength - (answerOne?.Length ?? 0))
                .ToProperty(
                    this,
                    vw => vw.AnswerOneLengthRemaining);

            _answerTwoLengthRemaining = this.WhenChanged(vm => vm.AnswerTwo)
                .Select(answerTwo => _maxLength - (answerTwo?.Length ?? 0))
                .ToProperty(
                    this,
                    vw => vw.AnswerTwoLengthRemaining);

            _answerThreeLengthRemaining = this.WhenChanged(vm => vm.AnswerThree)
                .Select(answerThree => _maxLength - (answerThree?.Length ?? 0))
                .ToProperty(
                    this,
                    vw => vw.AnswerThreeLengthRemaining);

            _answerFourLengthRemaining = this.WhenChanged(vm => vm.AnswerFour)
                .Select(answerFour => _maxLength - (answerFour?.Length ?? 0))
                .ToProperty(
                    this,
                    vw => vw.AnswerFourLengthRemaining);

            _answerFiveLengthRemaining = this.WhenChanged(vm => vm.AnswerFive)
                .Select(answerFive => _maxLength - (answerFive?.Length ?? 0))
                .ToProperty(
                    this,
                    vw => vw.AnswerFiveLengthRemaining);

            LaunchInteraction = ReactiveCommand.CreateFromTask(() => OnLaunchInteraction());
        }

        /// <summary>
        /// Gets or sets the forename.
        /// </summary>
        public string? Forename
        {
            get;
            set => this.RaiseAndSetIfChanged(ref field, value);
        }

        /// <summary>
        /// Gets the available length remaining on the forename.
        /// </summary>
        public int ForenameLengthRemaining => _forenameLengthRemaining.Value;

        /// <summary>
        /// Gets the command to launch an interaction.
        /// </summary>
        public ReactiveCommand<RxVoid, RxVoid> LaunchInteraction { get; }

        /// <summary>
        /// Gets or sets the surname.
        /// </summary>
        public string? Surname
        {
            get;
            set => this.RaiseAndSetIfChanged(ref field, value);
        }

        /// <summary>
        /// Gets the available length remaining on the surname.
        /// </summary>
        public int SurnameLengthRemaining => _surnameLengthRemaining.Value;

        /// <summary>
        /// Gets or sets the first answer.
        /// </summary>
        public string? AnswerOne
        {
            get;
            set => this.RaiseAndSetIfChanged(ref field, value);
        }

        /// <summary>
        /// Gets the available length remaining on answer one.
        /// </summary>
        public int AnswerOneLengthRemaining => _answerOneLengthRemaining.Value;

        /// <summary>
        /// Gets or sets the second answer.
        /// </summary>
        public string? AnswerTwo
        {
            get;
            set => this.RaiseAndSetIfChanged(ref field, value);
        }

        /// <summary>
        /// Gets the available length remaining on answer two.
        /// </summary>
        public int AnswerTwoLengthRemaining => _answerTwoLengthRemaining.Value;

        /// <summary>
        /// Gets or sets the third answer.
        /// </summary>
        public string? AnswerThree
        {
            get;
            set => this.RaiseAndSetIfChanged(ref field, value);
        }

        /// <summary>
        /// Gets the available length remaining on answer three.
        /// </summary>
        public int AnswerThreeLengthRemaining => _answerThreeLengthRemaining.Value;

        /// <summary>
        /// Gets or sets the fourth answer.
        /// </summary>
        public string? AnswerFour
        {
            get;
            set => this.RaiseAndSetIfChanged(ref field, value);
        }

        /// <summary>
        /// Gets the available length remaining on answer four.
        /// </summary>
        public int AnswerFourLengthRemaining => _answerFourLengthRemaining.Value;

        /// <summary>
        /// Gets or sets the fourth answer.
        /// </summary>
        public string? AnswerFive
        {
            get;
            set => this.RaiseAndSetIfChanged(ref field, value);
        }

        /// <summary>
        /// Gets the available length remaining on answer five.
        /// </summary>
        public int AnswerFiveLengthRemaining => _answerFiveLengthRemaining.Value;

        /// <summary>
        /// Gets the maximum length.
        /// </summary>
        /// <remarks>
        /// This is here for demonstration purposes. In this scenario it is actually a constant value for all text inputs.
        /// However in a more complete capture, each input may vary, this is here to allow you to see how to hook
        /// the bindings together, and fire off relevant logic.
        /// </remarks>
        public int MaxLength => _maxLength;

        private static Task<RxVoid> OnLaunchInteraction()
        {
            return Task.FromResult(RxVoid.Default);
        }
    }
}
