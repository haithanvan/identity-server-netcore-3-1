using System;
using System.Collections.Generic;
using System.Text;

namespace Nmb.Shared.Quiz
{
    public class CorrectAnswer
    {
        public Guid Id { get; set; }
        public string Value { get; set; }
        public int Point { get; set; }
    }
}
