using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIMS.Application.DTOs.Students;
using SIMS.Application.Interfaces.Services;
using SIMS.Domain.Constants;
using SIMS.Shared.Models;

namespace SIMS_BackEnd.Controllers;

[ApiController]
[Route("api/students")]
[Authorize]
public class StudentsController : ControllerBase
{
    private readonly IStudentService _studentService;

    public StudentsController(IStudentService studentService)
    {
        _studentService = studentService;
    }

    /// <summary>Returns all students with name and email resolved from the linked user.</summary>
    [HttpGet]
    [Authorize(Policy = Permissions.ViewStudents)]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<StudentResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAllStudents(CancellationToken ct)
    {
        var students = await _studentService.GetAllAsync(ct);
        return Ok(ApiResponse<IEnumerable<StudentResponse>>.Ok(students));
    }

    /// <summary>Returns a single student by their internal ID.</summary>
    [HttpGet("{id:int}")]
    [Authorize(Policy = Permissions.ViewStudents)]
    [ProducesResponseType(typeof(ApiResponse<StudentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStudent(int id, CancellationToken ct)
    {
        var student = await _studentService.GetByIdAsync(id, ct);
        return Ok(ApiResponse<StudentResponse>.Ok(student));
    }

    /// <summary>
    /// Creates a new student record.
    /// StudentCode must match the format BD followed by digits (e.g. BD00519).
    /// </summary>
    [HttpPost]
    [Authorize(Policy = Permissions.CreateStudent)]
    [ProducesResponseType(typeof(ApiResponse<StudentResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> CreateStudent([FromBody] CreateStudentRequest request, CancellationToken ct)
    {
        var student = await _studentService.CreateAsync(request, ct);
        return StatusCode(StatusCodes.Status201Created,
            ApiResponse<StudentResponse>.Ok(student, "Student created successfully."));
    }

    /// <summary>
    /// Bulk-imports student profiles from a CSV file.
    /// Expected columns (no header required): StudentCode, FirstName, LastName,
    /// DateOfBirth, Gender, Phone, City, Country, Email, Major.
    /// Rows that fail validation or have duplicate codes/emails are skipped and
    /// included in the response's Errors list.
    /// </summary>
    [HttpPost("import")]
    [Authorize(Policy = Permissions.ImportStudents)]
    [ProducesResponseType(typeof(ApiResponse<ImportStudentsResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status422UnprocessableEntity)]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> ImportStudents([FromForm] IFormFile? file, CancellationToken ct)
    {
        if (file is null || file.Length == 0)
            return BadRequest(ApiResponse.Fail(SIMS.Shared.Exceptions.ErrorCode.VALIDATION_ERROR,
                new[] { "No file was uploaded or the file is empty." }));

        var result = await _studentService.ImportAsync(file.OpenReadStream(), ct);
        return Ok(ApiResponse<ImportStudentsResponse>.Ok(result,
            $"Import complete: {result.Imported} imported, {result.Skipped} skipped."));
    }

    /// <summary>
    /// Updates an existing student record.
    /// If StudentCode is provided it must still match BD + digits.
    /// </summary>
    [HttpPut("{id:int}")]
    [Authorize(Policy = Permissions.EditStudent)]
    [ProducesResponseType(typeof(ApiResponse<StudentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> UpdateStudent(int id, [FromBody] UpdateStudentRequest request, CancellationToken ct)
    {
        var student = await _studentService.UpdateAsync(id, request, ct);
        return Ok(ApiResponse<StudentResponse>.Ok(student));
    }

    /// <summary>Deletes a student record by ID.</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Policy = Permissions.DeleteStudent)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteStudent(int id, CancellationToken ct)
    {
        await _studentService.DeleteAsync(id, ct);
        return Ok(ApiResponse<object?>.Ok(null));
    }
}
