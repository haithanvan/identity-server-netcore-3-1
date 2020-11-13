using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Nmb.Shared.SeedWork;
using Nmb.Shared.Serializations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace Nmb.Shared.Quiz
{
    public class Question
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public string Instruction { get; set; }
        public int QuestionPoint { get; set; }        
        public IList<ChoiceAnswer> Choices { get; set; }
        [JsonIgnore]
        [NotMapped]
        public bool AllowMultiple => (Choices != null && Choices.Count > 0 && Choices.Count(t => t.CorrectAnswer) > 1) ? true : false;
    }
}