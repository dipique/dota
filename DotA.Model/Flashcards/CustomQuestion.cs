namespace DotA.Flashcards
{
    public class CustomQuestion
    {
        public string QuestionText { get; set; }
        public string CorrectAnswer { get; set; }
        public string WrongAnswer1 { get; set; }
        public string WrongAnswer2 { get; set; }
        public string WrongAnswer3 { get; set; }
        public QuestionType Type { get; set; }
    }
}
