using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FormBuilder.Models
{
    public class Answer
    {
        [Key]
        public int Id { get; set; }


        [Required]
        public Guid UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        [Required]
        public int FormElementId { get; set; }

        [ForeignKey("FormElementId")]
        public FormElement FormElement { get; set; }

        [Required]
        public string AnswerText { get; set; }

        public Guid? EvaluatedUserId { get; set; }

        [ForeignKey("EvaluatedUserId")]
        public User EvaluatedUser { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}


