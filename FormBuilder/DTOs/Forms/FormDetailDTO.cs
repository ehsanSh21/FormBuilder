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


        public List<FormGroupDTO> FormGroups { get; set; }
    }

    public class FormGroupDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Data { get; set; }


        public List<FormElementDTO> FormElements { get; set; }
    }

    public class FormElementDTO
    {
        public string Title { get; set; }
        public string Type { get; set; }
        public int Ordering { get; set; }

        public decimal? OverallPoint { get; set; }

        //public FormElementResultDTO Result { get; set; }
        public List<FormElementResultDTO> FormElementResults { get; set; }

        public List<AnswerDTO> Answers { get; set; }

        public List<OptionDTO> Options { get; set; } = new List<OptionDTO>
            {
                new OptionDTO { Label = "Very Bad", Value = 1 },
                new OptionDTO { Label = "Bad", Value = 2 },
                new OptionDTO { Label = "Natural", Value = 3 },
                new OptionDTO { Label = "Good", Value = 4 },
                new OptionDTO { Label = "Very Good", Value = 5 }
            };

    }

    public class FormElementResultDTO
    {
        public decimal? OverallPoint { get; set; }
        //public int Id { get; set; }

        // Other properties related to the result
    }

    public class OptionDTO
    {
        public string Label { get; set; }
        public int Value { get; set; }
    }

    public class AnswerDTO
    {

        public Guid UserId { get; set; }

        public int FormElementId { get; set; }

        public string AnswerText { get; set; }

        public Guid? EvaluatedUserId { get; set; }
    }

}
