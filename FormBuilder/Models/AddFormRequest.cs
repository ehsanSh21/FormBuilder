using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace FormBuilder.Models
{
    //public class AddFormRequest
    //{
    //    public string Type { get; set; }
    //    public Guid? UserId { get; set; }
    //}
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class RequiredIfTypeIsAssessmentAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var instance = validationContext.ObjectInstance as AddFormRequest;

            if (instance != null && instance.Type == "assessment" && value == null)
            {
                return new ValidationResult(ErrorMessage ?? $"{validationContext.DisplayName} is required for assessments.");
            }

            return ValidationResult.Success;
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class RequiredIfAttribute : ValidationAttribute
    {
        private readonly string _dependentPropertyName;
        private readonly object _targetValue;

        public RequiredIfAttribute(string dependentPropertyName, object targetValue)
        {
            _dependentPropertyName = dependentPropertyName;
            _targetValue = targetValue;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var dependentProperty = validationContext.ObjectType.GetProperty(_dependentPropertyName);

            if (dependentProperty == null)
            {
                return new ValidationResult($"Unknown property: {_dependentPropertyName}");
            }

            var dependentValue = dependentProperty.GetValue(validationContext.ObjectInstance);

            if (dependentValue == null)
            {
                return ValidationResult.Success;
            }

            if (dependentValue.Equals(_targetValue) && value == null)
            {
                return new ValidationResult(ErrorMessage ?? $"{validationContext.DisplayName} is required.");
            }

            return ValidationResult.Success;
        }
    }

    public class AddFormRequest
{
        //public Dictionary<string, string> Translation { get; set; }

        //[Required(ErrorMessage = "The Title field is required.")]
        //public string TranslationFaTitle { get; set; }
    
     [Required(ErrorMessage = "The UserId field is required.")]

     public Guid? UserId { get; set; }

        [Required(ErrorMessage = "The Title field is required.")]
        public string Title { get; set; }


        [Required(ErrorMessage = "The Type field is required.")]
    [RegularExpression("^(exam|assessment)$", ErrorMessage = "Invalid Type. Must be 'exam' or 'assessment'.")]
    public string Type { get; set; }

    [RequiredIf("Type", "exam", ErrorMessage = "The Point field is required.")]
    [Range(0, int.MaxValue, ErrorMessage = "Point must be a non-negative integer.")]
    public int? Point { get; set; }

    [RequiredIf("Type", "exam", ErrorMessage = "The Min Point field is required for exams.")]
    [Range(0, int.MaxValue, ErrorMessage = "Min Point must be a non-negative integer.")]
    public int? MinPoint { get; set; }

    public List<FormGroupRequest> FormGroups { get; set; }

    public class FormGroupRequest
    {
        [Required(ErrorMessage = "Data field is required.")]
        public string Data { get; set; }

            [Required(ErrorMessage = "The Title field is required.")]
            public string Title { get; set; }

            //[Required(ErrorMessage = "Id field is required.")]
            //public int? Id { get; set; }

            //public Dictionary<string, string> Translation { get; set; }

            //[Required(ErrorMessage = "The Title field is required.")]
            //public string TranslationFaTitle { get; set; }


            //[RequiredIf("Type", "assessment", ErrorMessage = "Ratio field is required for assessments.")]
            //[Range(1, double.MaxValue, ErrorMessage = "Ratio must be a positive number.")]
            //public double? Ratio { get; set; }

            [RequiredIfTypeIsAssessment(ErrorMessage = "Ratio field is required for assessments.")]
            [Range(1, double.MaxValue, ErrorMessage = "Ratio must be a positive number.")]
            public double? Ratio { get; set; }


            public List<FormElementRequest> Elements { get; set; }

        public class FormElementRequest
        {
                //[UuidIfExists(ErrorMessage = "UUID must be a valid UUID if provided.")]
                //public string Uuid { get; set; }

            [Required(ErrorMessage = "The Title field is required.")]
          public string Title { get; set; }

                [Required(ErrorMessage = "Ordering field is required.")]
                public int? Ordering { get; set; }

                [Required(ErrorMessage = "Type field is required.")]
            [RegularExpression("^(select|text)$", ErrorMessage = "Invalid Type. Must be 'select' or 'text'.")]
            public string Type { get; set; }

            [RequiredIf("Type", "assessment", ErrorMessage = "Ratio field is required for assessments.")]
            [Range(1, double.MaxValue, ErrorMessage = "Ratio must be a positive number.")]
            public double? Ratio { get; set; }

            [RequiredIf("Type", "assessment", ErrorMessage = "Reverse Grading field is required for assessments.")]
            public bool? ReverseGrading { get; set; }

            //public Dictionary<string, string> Translation { get; set; }

            //[Required(ErrorMessage = "The Title field is required.")]
            //public string TranslationFaTitle { get; set; }

            [RequiredIf("Type", "exam", ErrorMessage = "Point field is required for exams.")]
            [Range(0, int.MaxValue, ErrorMessage = "Point must be a non-negative integer.")]
            public int? Point { get; set; }
        }
    }
}

}

