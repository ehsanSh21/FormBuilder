using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FormBuilder.Models
{
    public class FormElementResult
    {
            [Key]
            public int Id { get; set; }

            [Required]
            public int FormElementId { get; set; }

            [ForeignKey("User")]

            public Guid? UserId { get; set; }

            public User User { get; set; }


            [Required]
            public decimal OverallPoint { get; set; }

            [ForeignKey(nameof(FormElementId))]
            public FormElement FormElement { get; set; }

   
    }
}
