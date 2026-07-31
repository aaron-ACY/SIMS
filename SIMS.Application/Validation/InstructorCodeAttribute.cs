using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace SIMS.Application.Validation;

/// <summary>
/// Validates instructor code format: "GV" prefix followed by exactly 5 digits (e.g., GV00123).
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public partial class InstructorCodeAttribute : ValidationAttribute
{
    private const string Pattern = @"^GV\d{5}$";

    [GeneratedRegex(Pattern, RegexOptions.IgnoreCase)]
    private static partial Regex InstructorCodeRegex();

    public InstructorCodeAttribute()
        : base("Instructor code must start with 'GV' followed by exactly 5 digits (e.g., GV00123).")
    {
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null or "")
            return ValidationResult.Success; // Defer to [Required] if needed

        var code = value.ToString()!;

        if (!InstructorCodeRegex().IsMatch(code))
            return new ValidationResult(ErrorMessage);

        return ValidationResult.Success;
    }
}
