using System;
using System.Collections.Generic;
using System.Text;

namespace Nmb.Shared.Quiz
{
    public class ChoiceAnswer
    {
        public Guid? Id { get; set; } = Guid.NewGuid();
        public string Content { get; set; }
        public int Point { get; set; }
        public bool CorrectAnswer { get; set; } = false;
    }
}
