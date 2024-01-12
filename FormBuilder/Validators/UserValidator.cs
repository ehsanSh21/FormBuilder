using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using FormBuilder.Models;

namespace FormBuilder.Validators
{
    public class UserValidator: AbstractValidator<User>
    {
        public UserValidator()
        {
            RuleFor(user => user.FullName).NotEmpty().WithMessage("Full Name is required");
            RuleFor(user => user.Email).NotEmpty().WithMessage("Last Name is required");
            RuleFor(user => user.Phone).InclusiveBetween(1, 150).WithMessage("Age must be between 1 and 150");
            RuleFor(user => user.Address).NotEmpty().WithMessage("Age must be between 1 and 150");
            // Add more rules as needed
        }
    }
}
