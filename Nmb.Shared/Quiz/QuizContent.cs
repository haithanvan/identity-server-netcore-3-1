using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Nmb.Shared.Quiz
{
    public class QuizContent
    {
        public IList<Question> Questions { get; set; }
        [JsonIgnore]
        [NotMapped]
        public virtual int NumberOfQuestions => Questions != null && Questions.Count > 0 ? Questions.Count : 0;
        [JsonConstructor]
        public QuizContent()
        {
        }

        public QuizContent(QuizContent content) : this()
        {
            Questions = content.Questions;
        }
    }
}
