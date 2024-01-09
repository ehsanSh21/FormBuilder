using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormBuilder.Models
{
    public class UpdateFormRequest
    {
        public string Type { get; set; }
        public Guid? UserId { get; set; }
    }
}
