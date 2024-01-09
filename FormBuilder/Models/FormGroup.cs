using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FormBuilder.Models
{   
    public class FormGroup
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Form")]
        public int FormId { get; set; }

        public Form Form { get; set; }

        [Required]
        public string Title { get; set; }

        public string Data { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }



        public List<FormElement> FormElements { get; set; }

    }
}
