using System.Globalization;
using System.Text;
using SIMS.Application.DTOs.Students;
using SIMS.Application.Interfaces.Repositories;
using SIMS.Application.Interfaces.Services;
using SIMS.Domain.Entities;
using SIMS.Shared.Exceptions;

namespace SIMS.Application.Services;

public class StudentService : IStudentService
{
    private readonly IStudentRepository _studentRepository;
    private readonly IUserRepository    _userRepository;

    public StudentService(
        IStudentRepository studentRepository,
        IUserRepository    userRepository)
    {
        _studentRepository = studentRepository;
        _userRepository    = userRepository;
    }

    public async Task<IEnumerable<StudentResponse>> GetAllAsync(CancellationToken ct = default)
    {
        var students = await _studentRepository.GetAllAsync();
        var users    = await _userRepository.GetAllAsync();
        var userMap  = users.ToDictionary(u => u.Id);

        return students.Select(s => Map(s, userMap));
    }

    public async Task<StudentResponse> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var student = await _studentRepository.GetByIdAsync(id)
                      ?? throw new AppException(ErrorCode.STUDENT_NOT_EXISTED);

        var user    = student.UserId.HasValue
            ? await _userRepository.GetByIdAsync(student.UserId.Value)
            : null;

        var userMap = user is null
            ? new Dictionary<int, User>()
            : new Dictionary<int, User> { [user.Id] = user };

        return Map(student, userMap);
    }

    public async Task<StudentResponse> CreateAsync(CreateStudentRequest request, CancellationToken ct = default)
    {
        // Validate linked user exists
        var user = await _userRepository.GetByIdAsync(request.UserId)
                   ?? throw new AppException(ErrorCode.USER_NOT_EXISTED);

        // StudentCode must be unique (case-insensitive)
        var existing = await _studentRepository.GetByStudentCodeAsync(request.StudentCode);
        if (existing is not null)
            throw new AppException(ErrorCode.STUDENT_CODE_EXISTED);

        var student = new Student
        {
            UserId         = request.UserId,
            FirstName      = user.FirstName,
            LastName       = user.LastName,
            Email          = user.Email,
            StudentCode    = request.StudentCode.ToUpperInvariant(),
            DateOfBirth    = request.DateOfBirth,
            Gender         = request.Gender.Trim(),
            Phone          = request.Phone.Trim(),
            Address        = request.Address.Trim(),
            Major          = request.Major.Trim(),
            EnrollmentYear = request.EnrollmentYear,
            Status         = request.Status.Trim(),
            IsActive       = true
        };

        await _studentRepository.AddAsync(student);

        return Map(student, new Dictionary<int, User> { [user.Id] = user });
    }

    public async Task<StudentResponse> UpdateAsync(int id, UpdateStudentRequest request, CancellationToken ct = default)
    {
        var student = await _studentRepository.GetByIdAsync(id)
                      ?? throw new AppException(ErrorCode.STUDENT_NOT_EXISTED);

        // If changing StudentCode, ensure it stays unique
        if (request.StudentCode is not null)
        {
            var conflict = await _studentRepository.GetByStudentCodeAsync(request.StudentCode);
            if (conflict is not null && conflict.Id != id)
                throw new AppException(ErrorCode.STUDENT_CODE_EXISTED);

            student.StudentCode = request.StudentCode.ToUpperInvariant();
        }

        if (request.DateOfBirth.HasValue)    student.DateOfBirth    = request.DateOfBirth.Value;
        if (request.Gender     is not null)  student.Gender         = request.Gender.Trim();
        if (request.Phone      is not null)  student.Phone          = request.Phone.Trim();
        if (request.Address    is not null)  student.Address        = request.Address.Trim();
        if (request.Major      is not null)  student.Major          = request.Major.Trim();
        if (request.EnrollmentYear.HasValue) student.EnrollmentYear = request.EnrollmentYear.Value;
        if (request.Status     is not null)  student.Status         = request.Status.Trim();

        await _studentRepository.UpdateAsync(student);

        var user    = student.UserId.HasValue
            ? await _userRepository.GetByIdAsync(student.UserId.Value)
            : null;
        var userMap = user is null
            ? new Dictionary<int, User>()
            : new Dictionary<int, User> { [user.Id] = user };

        return Map(student, userMap);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var deleted = await _studentRepository.DeleteAsync(id);
        if (!deleted)
            throw new AppException(ErrorCode.STUDENT_NOT_EXISTED);
    }

    /// <inheritdoc/>
    public async Task<ImportStudentsResponse> ImportAsync(Stream csvStream, CancellationToken ct = default)
    {
        var errors   = new List<string>();
        int imported = 0;
        int skipped  = 0;
        int total    = 0;

        using var reader = new StreamReader(csvStream, Encoding.UTF8);

        // ── Step 1: read all non-empty lines ──────────────────────────── //
        var lines = new List<string>();
        string? raw;
        while ((raw = await reader.ReadLineAsync()) != null)
            if (!string.IsNullOrWhiteSpace(raw))
                lines.Add(raw);

        if (lines.Count == 0)
            return new ImportStudentsResponse { Errors = ["File is empty."] };

        // ── Step 2: auto-detect delimiter from first line ─────────────── //
        var delimiter = DetectDelimiter(lines[0]);

        // ── Step 3: process rows ──────────────────────────────────────── //
        int rowNumber = 0;
        foreach (var line in lines)
        {
            rowNumber++;
            var cols = SplitLine(line, delimiter);

            // Skip header: first column is not a student code (e.g. "Student Code")
            if (rowNumber == 1 &&
                !System.Text.RegularExpressions.Regex.IsMatch(cols[0].Trim(), @"^[A-Za-z]{2}\d+$"))
                continue;

            total++;

            // Accept 9 columns (Address combined) OR 10 columns (City + Country).
            // Layout 9:  StudentCode, FirstName, LastName, DOB, Gender, Phone, Address, Email, Major
            // Layout 10: StudentCode, FirstName, LastName, DOB, Gender, Phone, City, Country, Email, Major
            if (cols.Length < 9)
            {
                skipped++;
                errors.Add(
                    $"Row {rowNumber}: expected at least 9 columns " +
                    $"(StudentCode, FirstName, LastName, DateOfBirth, Gender, Phone, Address, Email, Major), " +
                    $"found {cols.Length}. Check that the delimiter is comma, semicolon, or tab.");
                continue;
            }

            string studentCode, firstName, lastName, dobRaw, gender, phone, address, email, major;

            if (cols.Length >= 10)
            {
                // 10-column layout: City and Country are separate.
                studentCode = cols[0].Trim().ToUpperInvariant();
                firstName   = cols[1].Trim();
                lastName    = cols[2].Trim();
                dobRaw      = cols[3].Trim();
                gender      = cols[4].Trim();
                phone       = cols[5].Trim();
                var city    = cols[6].Trim();
                var country = cols[7].Trim();
                address     = string.Join(", ", new[] { city, country }.Where(s => !string.IsNullOrEmpty(s)));
                email       = cols[8].Trim().ToLowerInvariant();
                major       = cols[9].Trim();
            }
            else
            {
                // 9-column layout: Address already combined.
                studentCode = cols[0].Trim().ToUpperInvariant();
                firstName   = cols[1].Trim();
                lastName    = cols[2].Trim();
                dobRaw      = cols[3].Trim();
                gender      = cols[4].Trim();
                phone       = cols[5].Trim();
                address     = cols[6].Trim();
                email       = cols[7].Trim().ToLowerInvariant();
                major       = cols[8].Trim();
            }

            // Validate required fields.
            var rowErrors = new List<string>();
            if (string.IsNullOrEmpty(studentCode)) rowErrors.Add("StudentCode is required.");
            if (string.IsNullOrEmpty(firstName))   rowErrors.Add("FirstName is required.");
            if (string.IsNullOrEmpty(lastName))    rowErrors.Add("LastName is required.");
            if (string.IsNullOrEmpty(email))       rowErrors.Add("Email is required.");
            if (string.IsNullOrEmpty(major))       rowErrors.Add("Major is required.");

            if (!DateTime.TryParse(dobRaw, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateOfBirth))
                rowErrors.Add($"DateOfBirth '{dobRaw}' is not a valid date (expected YYYY-MM-DD).");

            if (rowErrors.Count > 0)
            {
                skipped++;
                errors.Add($"Row {rowNumber} [{studentCode}]: {string.Join(" ", rowErrors)}");
                continue;
            }

            // Uniqueness checks.
            if (await _studentRepository.GetByStudentCodeAsync(studentCode) is not null)
            {
                skipped++;
                errors.Add($"Row {rowNumber}: StudentCode '{studentCode}' already exists — skipped.");
                continue;
            }

            if (await _studentRepository.GetByEmailAsync(email) is not null)
            {
                skipped++;
                errors.Add($"Row {rowNumber}: Email '{email}' already exists — skipped.");
                continue;
            }

            var student = new Student
            {
                UserId         = null,
                StudentCode    = studentCode,
                FirstName      = firstName,
                LastName       = lastName,
                Email          = email,
                DateOfBirth    = DateTime.SpecifyKind(dateOfBirth, DateTimeKind.Utc),
                Gender         = gender,
                Phone          = phone,
                Address        = address,
                Major          = major,
                EnrollmentYear = DateTime.UtcNow.Year,
                Status         = "Active",
                IsActive       = true
            };

            await _studentRepository.AddAsync(student);
            imported++;
        }

        return new ImportStudentsResponse
        {
            TotalRows = total,
            Imported  = imported,
            Skipped   = skipped,
            Errors    = errors
        };
    }

    // ── Helpers ────────────────────────────────────────────────────────── //

    /// <summary>
    /// Infers the field delimiter by counting candidate chars in a sample line.
    /// Checks tab, semicolon and comma in that order; the one with the highest
    /// count wins (ties go to the first in the list).
    /// </summary>
    private static char DetectDelimiter(string sampleLine)
    {
        char[] candidates = ['\t', ';', ','];
        return candidates
            .OrderByDescending(d => sampleLine.Count(c => c == d))
            .First();
    }

    /// <summary>
    /// Splits a line on <paramref name="delimiter"/>, respecting double-quoted fields.
    /// Outer quotes are stripped; doubled inner quotes ("") are un-escaped.
    /// </summary>
    private static string[] SplitLine(string line, char delimiter)
    {
        var result   = new List<string>();
        var current  = new StringBuilder();
        bool inQuote = false;

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];

            if (c == '"')
            {
                if (inQuote && i + 1 < line.Length && line[i + 1] == '"')
                {
                    current.Append('"');
                    i++;
                }
                else
                {
                    inQuote = !inQuote;
                }
            }
            else if (c == delimiter && !inQuote)
            {
                result.Add(current.ToString());
                current.Clear();
            }
            else
            {
                current.Append(c);
            }
        }

        result.Add(current.ToString());
        return result.ToArray();
    }

    private static StudentResponse Map(
        Student                student,
        Dictionary<int, User>  userMap)
    {
        // When UserId is set, prefer the User's identity; otherwise fall back
        // to the profile data that was imported from CSV.
        User? user = student.UserId.HasValue
            ? (userMap.TryGetValue(student.UserId.Value, out var u) ? u : null)
            : null;

        return new StudentResponse
        {
            UserId         = student.UserId,
            StudentCode    = student.StudentCode,
            FullName       = user?.FullName ?? $"{student.FirstName} {student.LastName}".Trim(),
            Email          = user?.Email    ?? student.Email,
            DateOfBirth    = student.DateOfBirth,
            Gender         = student.Gender,
            Phone          = student.Phone,
            Address        = student.Address,
            Major          = student.Major,
            EnrollmentYear = student.EnrollmentYear,
            Status         = student.Status,
            IsActive       = student.IsActive,
            IsRegistered   = student.UserId.HasValue
        };
    }
}
