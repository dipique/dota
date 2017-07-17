using System;


namespace DotA.Flashcards
{
    public class QuestionResponse
    {
        public DateTime QuestionDate { get; set; }
        public QuestionType Type { get; set; }
        
        public string QuestionText { get; set; }
        public bool CorrectAnswer { get; set; }
        public string CorrectAnswerText { get; set; }
        public string UserAnswerText { get; set; }
    }

    /// <summary>
    /// Hierarchies are separated by double underscores (__). Single
    /// underscores will be interpreted as a space.
    /// </summary>
    [Flags]
    public enum QuestionType
    {
        Hero__Stats = 1,
        Hero__Skills__Hero_Match = 1 << 1,
        Hero__Skills__Behavior = 1 << 2,
        Hero__Talents = 1 << 3,
        Hero__Other = 1 << 4,
        Item__Recipe = 1 << 5,
        Item__Behavior = 1 << 6,
        Item__Cost = 1 << 7,
        Item__Other = 1 << 8,
        Mechanics = 1 << 9,
        All = ~0
    }
}
