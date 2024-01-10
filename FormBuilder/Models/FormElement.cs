using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FormBuilder.Data;
using Microsoft.EntityFrameworkCore;

namespace FormBuilder.Models
{
    public class FormElement
    {
        [Key]
        public int Id { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Uuid { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Type { get; set; }

        public int? FormId { get; set; }

        [ForeignKey("FormId")]
        public Form Form { get; set; }

        public int? GroupId { get; set; }

        [ForeignKey("GroupId")]
        public FormGroup FormGroup { get; set; }

        public int Ordering { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }



        public List<Answer> Answers { get; set; }


        public List<FormElementResult> FormElementResults { get; set; }



    }
}
