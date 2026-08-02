using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace SIMS.Application.Validation;

/// <summary>
/// Validates student code format: "BD" prefix followed by exactly 5 digits (e.g., BD00519).
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public partial class StudentCodeAttribute : ValidationAttribute
{
    private const string Pattern = @"^BD\d{5}$";

    [GeneratedRegex(Pattern, RegexOptions.IgnoreCase)]
    private static partial Regex StudentCodeRegex();

    public StudentCodeAttribute()
        : base("Student code must start with 'BD' followed by exactly 5 digits (e.g., BD00519).")
    {
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null or "")
            return ValidationResult.Success; // Defer to [Required] if needed

        var code = value.ToString()!;

        if (!StudentCodeRegex().IsMatch(code))
            return new ValidationResult(ErrorMessage);

        return ValidationResult.Success;
    }
}
