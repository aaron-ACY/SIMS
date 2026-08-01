using System.ComponentModel.DataAnnotations;
using FluentAssertions;
using SIMS.Application.Validation;
using Xunit;

namespace SIMS.Tests.Validation;

public class StudentCodeAttributeTests
{
    private static ValidationResult? Validate(string? code)
    {
        var attr    = new StudentCodeAttribute();
        var context = new ValidationContext(new object());
        return attr.GetValidationResult(code, context);
    }


    [Fact]
    public void IsValid_WithValidCode_ShouldReturnSuccess()
    {
        // Arrange
        const string validCode = "BD00519";

        // Act
        var result = Validate(validCode);

        // Assert
        result.Should().Be(ValidationResult.Success);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void IsValid_WithNullOrEmptyCode_ShouldDeferToRequired(string? code)
    {
        // Act
        var result = Validate(code);

        // Assert
        result.Should().Be(ValidationResult.Success);
    }

}
