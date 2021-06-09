using System;

namespace ReactiveUI.WPF.SampleApp.ViewModels
{
    public sealed class QuestionnaireViewModel : ReactiveObject
    {
        public string Forename { get; set; }

        public string Surname { get; set; }

        public string AnswerOne { get; set; }

        public string AnswerTwo { get; set; }

        public string AnswerThree { get; set; }

        public string AnswerFour { get; set; }

        public string AnswerFive { get; set; }

        public int MaxLength => 50;
    }
}
