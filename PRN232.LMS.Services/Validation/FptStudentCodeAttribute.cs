using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace PRN232.LMS.Services.Validation;

public class FptStudentCodeAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
        {
            return new ValidationResult("Student code is required.");
        }

        var code = value.ToString()!;
        
        // Pattern for SE19886, CE18793, HE181986 etc.
        // Usually starts with 2 capital letters (SE, CE, IA, GD, MC, etc.) and followed by 5 or 6 digits
        var regex = new Regex(@"^[A-Z]{2}\d{5,6}$");
        if (!regex.IsMatch(code))
        {
            return new ValidationResult("Student code must start with 2 uppercase letters followed by 5 or 6 digits (e.g., SE181986, CE19886).");
        }

        return ValidationResult.Success;
    }
}
