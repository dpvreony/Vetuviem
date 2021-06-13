using System;
using System.Linq.Expressions;

namespace ReactiveUI.WPF.SampleApp.ViewModels
{
    public sealed class QuestionnaireViewModel : ReactiveObject
    {
        private string _forename;
        private readonly ObservableAsPropertyHelper<int> _forenameLengthRemaining;

        private string _surname;
        private readonly ObservableAsPropertyHelper<int> _surnameLengthRemaining;

        private string _answerOne;
        private readonly ObservableAsPropertyHelper<int> _answerOneLengthRemaining;

        private string _answerTwo;
        private readonly ObservableAsPropertyHelper<int> _answerTwoLengthRemaining;

        private string _answerThree;
        private readonly ObservableAsPropertyHelper<int> _answerThreeLengthRemaining;

        private string _answerFour;
        private readonly ObservableAsPropertyHelper<int> _answerFourLengthRemaining;

        private string _answerFive;
        private readonly ObservableAsPropertyHelper<int> _answerFiveLengthRemaining;

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
        }

        public string Forename
        {
            get => _forename;
            set => this.RaiseAndSetIfChanged(ref _forename, value);
        }

        public int ForenameLengthRemaining => this._forenameLengthRemaining.Value;

        public string Surname
        {
            get => _surname;
            set => this.RaiseAndSetIfChanged(ref _surname, value);
        }

        public int SurnameLengthRemaining => this._surnameLengthRemaining.Value;

        public string AnswerOne
        {
            get => _answerOne;
            set => this.RaiseAndSetIfChanged(ref _answerOne, value);
        }

        public int AnswerOneLengthRemaining => this._answerOneLengthRemaining.Value;

        public string AnswerTwo
        {
            get => _answerTwo;
            set => this.RaiseAndSetIfChanged(ref _answerTwo, value);
        }

        public int AnswerTwoLengthRemaining => this._answerTwoLengthRemaining.Value;

        public string AnswerThree
        {
            get => _answerThree;
            set => this.RaiseAndSetIfChanged(ref _answerThree, value);
        }

        public int AnswerThreeLengthRemaining => this._answerThreeLengthRemaining.Value;

        public string AnswerFour
        {
            get => _answerFour;
            set => this.RaiseAndSetIfChanged(ref _answerFour, value);
        }

        public int AnswerFourLengthRemaining => this._answerFourLengthRemaining.Value;

        public string AnswerFive
        {
            get => _answerFive;
            set => this.RaiseAndSetIfChanged(ref _answerFive, value);
        }

        public int AnswerFiveLengthRemaining => this._answerFiveLengthRemaining.Value;

        public int MaxLength => 50;

        private ObservableAsPropertyHelper<int> GetLengthRemainingObservable(
            Expression<Func<QuestionnaireViewModel, string>> stringInputExpression,
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
