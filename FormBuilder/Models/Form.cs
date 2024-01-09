using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FormBuilder.Models
{
    public class Form
    {
        [Key]
        public int Id { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Uuid { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Type { get; set; }

        [ForeignKey("User")]
        
        public Guid? UserId { get; set; }  

        public User User { get; set; }


        [Timestamp]
        public byte[] Timestamp { get; set; }


        public List<FormGroup> FormGroups { get; set; }

        public List<FormElement> FormElements { get; set; }


    }
}
