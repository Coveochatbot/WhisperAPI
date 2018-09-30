using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WhisperAPI.Models
{
    public class QuestionToClient
    {
        public Guid Id { get; set; }

        public string Text { get; set; }

        public static QuestionToClient FromQuestion(Question question)
        {
            return new QuestionToClient
            {
                Id = question.Id,
                Text = question.Text
            };
        }
    }
}
