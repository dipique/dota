using System;
using System.Collections.Generic;
using System.Text;

using DotA.Model;

namespace DotA.Flashcards
{
    public class FlashcardMode
    {
        public DateTime LastUsed { get; set; } = DateTime.Now;
        public string Name { get; set; } //TODO: auto-create, but allow user to edit
        public QuestionType QuestionTypes { get; set; } = QuestionType.All;
        public List<Hero> Heroes { get; set; } //empty means include all heroes
        public bool Pinned { get; set; }
    }
}
