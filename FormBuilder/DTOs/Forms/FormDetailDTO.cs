using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormBuilder.DTOs.Forms
{
    // FormDetailDTO.cs
    public class FormDetailDTO
    {
        public int Id { get; set; }
        public Guid Uuid { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }

        public string UserFullName { get; set; } // Include FullName directly
        //public string UserName { get; set; }


        // Other properties as needed

        //public UserDTO User { get; set; } // Assuming you have a UserDTO for displaying user details

        public List<FormGroupDTO> FormGroups { get; set; }
    }

    public class FormGroupDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Data { get; set; }


        // Other properties as needed

        public List<FormElementDTO> FormElements { get; set; }
    }

    public class FormElementDTO
    {
        //public int Id { get; set; }
        //public Guid Uuid { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public int Ordering { get; set; }

        // Other properties as needed

        public decimal? OverallPoint { get; set; }

        public List<AnswerDTO> Answers { get; set; }
    }

    public class AnswerDTO
    {
        // Add properties from your Answer model

        public Guid UserId { get; set; }

        public int FormElementId { get; set; }

        public string AnswerText { get; set; }

        public Guid? EvaluatedUserId { get; set; }
    }

}
