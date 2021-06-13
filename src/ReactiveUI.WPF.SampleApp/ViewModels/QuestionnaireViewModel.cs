using System;

namespace ReactiveUI.WPF.SampleApp.ViewModels
{
    public sealed class QuestionnaireViewModel : ReactiveObject
    {
        private string _forename;
        private string _surname;
        private string _answerOne;
        private string _answerTwo;
        private string _answerThree;
        private string _answerFour;
        private string _answerFive;


        public string Forename
        {
            get => _forename;
            set => this.RaiseAndSetIfChanged(ref _forename, value);
        }

        public string Surname
        {
            get => _surname;
            set => this.RaiseAndSetIfChanged(ref _surname, value);
        }

        public string AnswerOne
        {
            get => _answerOne;
            set => this.RaiseAndSetIfChanged(ref _answerOne, value);
        }

        public string AnswerTwo
        {
            get => _answerTwo;
            set => this.RaiseAndSetIfChanged(ref _answerTwo, value);
        }

        public string AnswerThree
        {
            get => _answerThree;
            set => this.RaiseAndSetIfChanged(ref _answerThree, value);
        }

        public string AnswerFour
        {
            get => _answerFour;
            set => this.RaiseAndSetIfChanged(ref _answerFour, value);
        }

        public string AnswerFive
        {
            get => _answerFive;
            set => this.RaiseAndSetIfChanged(ref _answerFive, value);
        }

        public int MaxLength => 50;
    }
}
