using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FormBuilder.Models
{
    public class Meta
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Key { get; set; }

        public string Value { get; set; }

        public bool IsJson { get; set; }

        public string RelatableType { get; set; }

        public int RelatableId { get; set; }

    }
}
