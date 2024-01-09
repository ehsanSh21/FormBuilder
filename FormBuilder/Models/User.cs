using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FormBuilder.Models
{
    public class User
    {
        //public int Id { get; set; }
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public long Phone { get; set; }
        public string Address { get; set; }

        public List<Form> Forms { get; set; }

        public List<Answer> Answers { get; set; }

        // User evaluations
        [InverseProperty("EvaluatedUser")]
        public List<Answer> EvaluationsReceived { get; set; }

    }
}
