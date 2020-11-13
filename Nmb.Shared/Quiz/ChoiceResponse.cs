using System;
using System.Collections.Generic;
using System.Text;

namespace Nmb.Shared.Quiz
{
    public class ChoiceResponse
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public string Answer { get; set; }
    }
}
