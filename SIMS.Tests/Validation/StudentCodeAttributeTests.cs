using System.ComponentModel.DataAnnotations;
using FluentAssertions;
using SIMS.Application.Validation;
using Xunit;

namespace SIMS.Tests.Validation;

/// <summary>
/// Unit tests cho StudentCodeAttribute.
/// Quy tắc: phải bắt đầu bằng "BD" + đúng 5 chữ số (ví dụ: BD00519).
/// Regex: ^BD\d{5}$ (case-insensitive).
/// </summary>
public class StudentCodeAttributeTests
{
    private static ValidationResult? Validate(string? code)
    {
        var attr    = new StudentCodeAttribute();
        var context = new ValidationContext(new object());
        return attr.GetValidationResult(code, context);
    }

    // ── Hợp lệ ────────────────────────────────────────────────────────── //

    /// <summary>Mã đúng chuẩn: BD + 5 chữ số.</summary>
    [Theory]
    [InlineData("BD00519")]
    [InlineData("BD00001")]
    [InlineData("BD99999")]
    [InlineData("bd00519")]   // regex IgnoreCase → chấp nhận chữ thường
    [InlineData("Bd12345")]
    public void IsValid_WhenCodeMatchesBdFormat_ShouldReturnSuccess(string code)
    {
        // Act
        var result = Validate(code);

        // Assert
        result.Should().Be(ValidationResult.Success);
    }

    /// <summary>
    /// Null và chuỗi rỗng được coi là hợp lệ ở đây — lỗi "bắt buộc"
    /// do [Required] xử lý riêng, không phải nhiệm vụ của attribute này.
    /// </summary>
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void IsValid_WhenCodeIsNullOrEmpty_ShouldDeferValidationToRequired(string? code)
    {
        var result = Validate(code);
        result.Should().Be(ValidationResult.Success);
    }

    // ── Không hợp lệ ──────────────────────────────────────────────────── //

    /// <summary>Sai tiền tố — không bắt đầu bằng "BD".</summary>
    [Theory]
    [InlineData("ST00519")]   // Student → sai tiền tố
    [InlineData("SV00519")]   // Sinh Viên
    [InlineData("00519BD")]   // tiền tố ở cuối
    [InlineData("ABCDE")]
    public void IsValid_WhenCodeHasWrongPrefix_ShouldReturnValidationError(string code)
    {
        var result = Validate(code);
        result.Should().NotBe(ValidationResult.Success);
    }

    /// <summary>Số chữ số sau "BD" không đủ 5.</summary>
    [Theory]
    [InlineData("BD0051")]    // 4 chữ số
    [InlineData("BD005")]     // 3 chữ số
    [InlineData("BD1")]       // 1 chữ số
    [InlineData("BD")]        // không có chữ số
    public void IsValid_WhenCodeHasTooFewDigits_ShouldReturnValidationError(string code)
    {
        var result = Validate(code);
        result.Should().NotBe(ValidationResult.Success);
    }

    /// <summary>Số chữ số sau "BD" vượt quá 5.</summary>
    [Theory]
    [InlineData("BD005199")]  // 6 chữ số
    [InlineData("BD0051999")] // 7 chữ số
    public void IsValid_WhenCodeHasTooManyDigits_ShouldReturnValidationError(string code)
    {
        var result = Validate(code);
        result.Should().NotBe(ValidationResult.Success);
    }

    /// <summary>Phần sau "BD" chứa ký tự không phải số.</summary>
    [Theory]
    [InlineData("BDABC12")]
    [InlineData("BD0051A")]
    [InlineData("BD0051!")]
    [InlineData("BD 0051")]   // khoảng trắng bên trong
    public void IsValid_WhenCodeSuffixIsNonNumeric_ShouldReturnValidationError(string code)
    {
        var result = Validate(code);
        result.Should().NotBe(ValidationResult.Success);
    }

    /// <summary>Kiểm tra message lỗi khi không hợp lệ.</summary>
    [Fact]
    public void IsValid_WhenCodeIsInvalid_ShouldReturnErrorMessageMentioningFormat()
    {
        var result = Validate("INVALID");

        result.Should().NotBe(ValidationResult.Success);
        result!.ErrorMessage.Should().Contain("BD");
    }
}
