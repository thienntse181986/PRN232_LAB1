using FluentValidation;
using PRN232.LMS.Services.Models.Request;

namespace PRN232.LMS.Services.Validation;

public class StudentCreateRequestValidator : AbstractValidator<StudentCreateRequest>
{
    public StudentCreateRequestValidator()
    {
        RuleFor(x => x.StudentCode)
            .NotEmpty().WithMessage("Student Code is required.")
            .Matches(@"^[A-Z]{2}\d{5,6}$").WithMessage("Student Code must start with 2 uppercase letters followed by 5 or 6 digits (e.g., SE181986).");

        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full Name is required.")
            .Length(3, 100).WithMessage("Full Name must be between 3 and 100 characters.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email address is required.")
            .MaximumLength(100).WithMessage("Email cannot exceed 100 characters.");

        RuleFor(x => x.DateOfBirth)
            .NotEmpty().WithMessage("Date of Birth is required.")
            .LessThan(DateTime.Now.AddYears(-15)).WithMessage("Student must be at least 15 years old.");
    }
}
