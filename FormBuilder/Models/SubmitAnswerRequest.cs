using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormBuilder.Models
{
    public class SubmitAnswerRequest
    {
        public Guid UserId { get; set; }
        public Guid? EvaluatedUserId { get; set; }

        public List<AnswerDto> Answers { get; set; }
    }

    public class AnswerDto
    {
        public int FormElementId { get; set; }
        public string Answer { get; set; }
    }
}
