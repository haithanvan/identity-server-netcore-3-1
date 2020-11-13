using Nmb.Shared.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nmb.Shared.Quiz
{
    public class QuestionType : Enumeration
    {
        public static QuestionType MultipleChoice = new QuestionType(1, "Multiple");
        public static QuestionType Checkbox = new QuestionType(2, "Checkbox");
        public QuestionType() { }
        public QuestionType(int id, string name) : base(id, name) { }
    }
}
