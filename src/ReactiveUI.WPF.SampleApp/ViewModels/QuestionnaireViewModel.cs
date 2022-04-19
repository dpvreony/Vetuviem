// Copyright (c) 2022 DPVreony and Contributors. All rights reserved.
// DPVreony and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Linq.Expressions;
using System.Reactive;
using System.Threading.Tasks;

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

        private string? _forename;

        private string? _surname;

        private string? _answerOne;

        private string? _answerTwo;

        private string? _answerThree;

        private string? _answerFour;

        private string? _answerFive;

        /// <summary>
        /// Initializes a new instance of the <see cref="QuestionnaireViewModel"/> class.
        /// </summary>
        public QuestionnaireViewModel()
        {
            _forenameLengthRemaining = GetLengthRemainingObservable(
                vm => vm.Forename,
                vm => vm.ForenameLengthRemaining);

            _surnameLengthRemaining = GetLengthRemainingObservable(
                vm => vm.Surname,
                vm => vm.SurnameLengthRemaining);

            _answerOneLengthRemaining = GetLengthRemainingObservable(
                vm => vm.AnswerOne,
                vm => vm.AnswerOneLengthRemaining);

            _answerTwoLengthRemaining = GetLengthRemainingObservable(
                vm => vm.AnswerTwo,
                vm => vm.AnswerTwoLengthRemaining);

            _answerThreeLengthRemaining = GetLengthRemainingObservable(
                vm => vm.AnswerThree,
                vm => vm.AnswerThreeLengthRemaining);

            _answerFourLengthRemaining = GetLengthRemainingObservable(
                vm => vm.AnswerFour,
                vm => vm.AnswerFourLengthRemaining);

            _answerFiveLengthRemaining = GetLengthRemainingObservable(
                vm => vm.AnswerFive,
                vm => vm.AnswerFiveLengthRemaining);

            LaunchInteraction = ReactiveCommand.CreateFromTask(() => OnLaunchInteraction());
        }

        /// <summary>
        /// Gets or sets the forename.
        /// </summary>
        public string? Forename
        {
            get => _forename;
            set => this.RaiseAndSetIfChanged(ref _forename, value);
        }

        public int ForenameLengthRemaining => _forenameLengthRemaining.Value;

        public ReactiveCommand<Unit, Unit> LaunchInteraction { get; set; }

        /// <summary>
        /// Gets or sets the surname.
        /// </summary>
        public string? Surname
        {
            get => _surname;
            set => this.RaiseAndSetIfChanged(ref _surname, value);
        }

        public int SurnameLengthRemaining => _surnameLengthRemaining.Value;

        /// <summary>
        /// Gets or sets the first answer.
        /// </summary>
        public string? AnswerOne
        {
            get => _answerOne;
            set => this.RaiseAndSetIfChanged(ref _answerOne, value);
        }

        public int AnswerOneLengthRemaining => _answerOneLengthRemaining.Value;

        /// <summary>
        /// Gets or sets the second answer.
        /// </summary>
        public string? AnswerTwo
        {
            get => _answerTwo;
            set => this.RaiseAndSetIfChanged(ref _answerTwo, value);
        }

        public int AnswerTwoLengthRemaining => _answerTwoLengthRemaining.Value;

        /// <summary>
        /// Gets or sets the third answer.
        /// </summary>
        public string? AnswerThree
        {
            get => _answerThree;
            set => this.RaiseAndSetIfChanged(ref _answerThree, value);
        }

        public int AnswerThreeLengthRemaining => _answerThreeLengthRemaining.Value;

        /// <summary>
        /// Gets or sets the fourth answer.
        /// </summary>
        public string? AnswerFour
        {
            get => _answerFour;
            set => this.RaiseAndSetIfChanged(ref _answerFour, value);
        }

        public int AnswerFourLengthRemaining => _answerFourLengthRemaining.Value;

        /// <summary>
        /// Gets or sets the fourth answer.
        /// </summary>
        public string? AnswerFive
        {
            get => _answerFive;
            set => this.RaiseAndSetIfChanged(ref _answerFive, value);
        }

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

        private static Task<Unit> OnLaunchInteraction()
        {
            return Task.FromResult(Unit.Default);
        }

        private ObservableAsPropertyHelper<int> GetLengthRemainingObservable(
            Expression<Func<QuestionnaireViewModel, string?>> stringInputExpression,
            Expression<Func<QuestionnaireViewModel, int>> targetPropertyExpression)
        {
            return this.WhenAnyValue(
                    stringInputExpression,
                    vm => vm.MaxLength,
                    (stringChange, maxLengthChange) => maxLengthChange - (stringChange?.Length ?? 0))
                .ToProperty(
                    this,
                    targetPropertyExpression);
        }
    }
}
