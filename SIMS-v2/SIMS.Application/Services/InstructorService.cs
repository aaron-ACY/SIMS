using System.Globalization;
using System.Text;
using SIMS.Application.DTOs.Instructors;
using SIMS.Application.Interfaces.Repositories;
using SIMS.Application.Interfaces.Services;
using SIMS.Domain.Entities;
using SIMS.Shared.Exceptions;

namespace SIMS.Application.Services;

public class InstructorService : IInstructorService
{
    private readonly IInstructorRepository _instructorRepository;
    private readonly IUserRepository       _userRepository;
    private readonly IClassRepository      _classRepository;

    public InstructorService(
        IInstructorRepository instructorRepository,
        IUserRepository       userRepository,
        IClassRepository      classRepository)
    {
        _instructorRepository = instructorRepository;
        _userRepository       = userRepository;
        _classRepository      = classRepository;
    }

    public async Task<IEnumerable<InstructorResponse>> GetAllAsync(CancellationToken ct = default)
    {
        var instructors = await _instructorRepository.GetAllAsync();
        var users       = await _userRepository.GetAllAsync();
        var userMap     = users.ToDictionary(u => u.Id);

        return instructors.Select(i => Map(i, userMap));
    }

    public async Task<InstructorResponse> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var instructor = await _instructorRepository.GetByIdAsync(id)
                         ?? throw new AppException(ErrorCode.INSTRUCTOR_NOT_EXISTED);

        var user    = instructor.UserId.HasValue
            ? await _userRepository.GetByIdAsync(instructor.UserId.Value)
            : null;
        var userMap = user is null
            ? new Dictionary<int, User>()
            : new Dictionary<int, User> { [user.Id] = user };

        return Map(instructor, userMap);
    }

    public async Task<InstructorResponse> CreateAsync(CreateInstructorRequest request, CancellationToken ct = default)
    {
        // Validate linked user exists.
        var user = await _userRepository.GetByIdAsync(request.UserId)
                   ?? throw new AppException(ErrorCode.USER_NOT_EXISTED);

        // InstructorCode must be unique (case-insensitive).
        if (await _instructorRepository.GetByInstructorCodeAsync(request.InstructorCode) is not null)
            throw new AppException(ErrorCode.INSTRUCTOR_CODE_EXISTED);

        var instructor = new Instructor
        {
            UserId         = request.UserId,
            FirstName      = user.FirstName,
            LastName       = user.LastName,
            Email          = user.Email,
            InstructorCode = request.InstructorCode.Trim().ToUpperInvariant(),
            Department     = request.Department.Trim(),
            Degree         = request.Degree.Trim(),
            Phone          = request.Phone.Trim(),
            IsActive       = true
        };

        await _instructorRepository.AddAsync(instructor);

        return Map(instructor, new Dictionary<int, User> { [user.Id] = user });
    }

    public async Task<InstructorResponse> UpdateAsync(int id, UpdateInstructorRequest request, CancellationToken ct = default)
    {
        var instructor = await _instructorRepository.GetByIdAsync(id)
                         ?? throw new AppException(ErrorCode.INSTRUCTOR_NOT_EXISTED);

        if (request.InstructorCode is not null)
        {
            var conflict = await _instructorRepository.GetByInstructorCodeAsync(request.InstructorCode);
            if (conflict is not null && conflict.Id != id)
                throw new AppException(ErrorCode.INSTRUCTOR_CODE_EXISTED);

            instructor.InstructorCode = request.InstructorCode.Trim().ToUpperInvariant();
        }

        if (request.Department is not null) instructor.Department = request.Department.Trim();
        if (request.Degree     is not null) instructor.Degree     = request.Degree.Trim();
        if (request.Phone      is not null) instructor.Phone      = request.Phone.Trim();

        instructor.UpdatedAt = DateTime.UtcNow;
        await _instructorRepository.UpdateAsync(instructor);

        var user    = instructor.UserId.HasValue
            ? await _userRepository.GetByIdAsync(instructor.UserId.Value)
            : null;
        var userMap = user is null
            ? new Dictionary<int, User>()
            : new Dictionary<int, User> { [user.Id] = user };

        return Map(instructor, userMap);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var activeClasses = (await _classRepository.GetAllAsync())
            .Any(c => c.InstructorId == id && c.IsActive);
        if (activeClasses)
            throw new AppException(ErrorCode.USER_HAS_ACTIVE_CLASSES);

        if (!await _instructorRepository.DeleteAsync(id))
            throw new AppException(ErrorCode.INSTRUCTOR_NOT_EXISTED);
    }

    /// <inheritdoc/>
    public async Task<ImportInstructorsResponse> ImportAsync(Stream csvStream, CancellationToken ct = default)
    {
        var errors   = new List<string>();
        int imported = 0;
        int skipped  = 0;
        int total    = 0;

        using var reader = new StreamReader(csvStream, Encoding.UTF8);

        var lines = new List<string>();
        string? raw;
        while ((raw = await reader.ReadLineAsync()) != null)
            if (!string.IsNullOrWhiteSpace(raw))
                lines.Add(raw);

        if (lines.Count == 0)
            return new ImportInstructorsResponse { Errors = ["File is empty."] };

        var delimiter = DetectDelimiter(lines[0]);

        int rowNumber = 0;
        foreach (var line in lines)
        {
            rowNumber++;
            var cols = SplitLine(line, delimiter);

            if (rowNumber == 1 &&
                !System.Text.RegularExpressions.Regex.IsMatch(cols[0].Trim(), @"^[A-Za-z]{2}\d+$"))
                continue;

            total++;

            // Accept 10 cols (Address combined) OR 11 cols (City + Country).
            // Layout 10: InstructorCode, FirstName, LastName, DOB, Gender, Phone, Address, Email, Department, Degree
            // Layout 11: InstructorCode, FirstName, LastName, DOB, Gender, Phone, City, Country, Email, Department, Degree
            if (cols.Length < 10)
            {
                skipped++;
                errors.Add(
                    $"Row {rowNumber}: expected at least 10 columns " +
                    $"(InstructorCode, FirstName, LastName, DateOfBirth, Gender, Phone, Address, Email, Department, Degree), " +
                    $"found {cols.Length}. Check that the delimiter is comma, semicolon, or tab.");
                continue;
            }

            string instructorCode, firstName, lastName, dobRaw, gender, phone, address, email, department, degree;

            if (cols.Length >= 11)
            {
                instructorCode = cols[0].Trim().ToUpperInvariant();
                firstName      = cols[1].Trim();
                lastName       = cols[2].Trim();
                dobRaw         = cols[3].Trim();
                gender         = cols[4].Trim();
                phone          = cols[5].Trim();
                var city       = cols[6].Trim();
                var country    = cols[7].Trim();
                address        = string.Join(", ", new[] { city, country }.Where(s => !string.IsNullOrEmpty(s)));
                email          = cols[8].Trim().ToLowerInvariant();
                department     = cols[9].Trim();
                degree         = cols[10].Trim();
            }
            else
            {
                instructorCode = cols[0].Trim().ToUpperInvariant();
                firstName      = cols[1].Trim();
                lastName       = cols[2].Trim();
                dobRaw         = cols[3].Trim();
                gender         = cols[4].Trim();
                phone          = cols[5].Trim();
                address        = cols[6].Trim();
                email          = cols[7].Trim().ToLowerInvariant();
                department     = cols[8].Trim();
                degree         = cols[9].Trim();
            }

            var rowErrors = new List<string>();
            if (string.IsNullOrEmpty(instructorCode)) rowErrors.Add("InstructorCode is required.");
            if (string.IsNullOrEmpty(firstName))      rowErrors.Add("FirstName is required.");
            if (string.IsNullOrEmpty(lastName))       rowErrors.Add("LastName is required.");
            if (string.IsNullOrEmpty(email))          rowErrors.Add("Email is required.");
            if (string.IsNullOrEmpty(department))     rowErrors.Add("Department is required.");
            if (string.IsNullOrEmpty(degree))         rowErrors.Add("Degree is required.");

            if (!DateTime.TryParse(dobRaw, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateOfBirth))
                rowErrors.Add($"DateOfBirth '{dobRaw}' is not a valid date (expected YYYY-MM-DD).");

            if (rowErrors.Count > 0)
            {
                skipped++;
                errors.Add($"Row {rowNumber} [{instructorCode}]: {string.Join(" ", rowErrors)}");
                continue;
            }

            if (await _instructorRepository.GetByInstructorCodeAsync(instructorCode) is not null)
            {
                skipped++;
                errors.Add($"Row {rowNumber}: InstructorCode '{instructorCode}' already exists — skipped.");
                continue;
            }

            if (!string.IsNullOrEmpty(email) &&
                await _instructorRepository.GetByEmailAsync(email) is not null)
            {
                skipped++;
                errors.Add($"Row {rowNumber}: Email '{email}' already exists — skipped.");
                continue;
            }

            var instructor = new Instructor
            {
                UserId         = null,
                InstructorCode = instructorCode,
                FirstName      = firstName,
                LastName       = lastName,
                Email          = email,
                Department     = department,
                Degree         = degree,
                Phone          = phone,
                IsActive       = true
            };

            await _instructorRepository.AddAsync(instructor);
            imported++;
        }

        return new ImportInstructorsResponse
        {
            TotalRows = total,
            Imported  = imported,
            Skipped   = skipped,
            Errors    = errors
        };
    }

    // ── Helpers ────────────────────────────────────────────────────────── //

    private static char DetectDelimiter(string sampleLine)
    {
        char[] candidates = ['\t', ';', ','];
        return candidates
            .OrderByDescending(d => sampleLine.Count(c => c == d))
            .First();
    }

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

    private static InstructorResponse Map(
        Instructor            instructor,
        Dictionary<int, User> userMap)
    {
        User? user = instructor.UserId.HasValue
            ? (userMap.TryGetValue(instructor.UserId.Value, out var u) ? u : null)
            : null;

        return new InstructorResponse
        {
            Id             = instructor.Id,
            InstructorCode = instructor.InstructorCode,
            FullName       = user?.FullName ?? $"{instructor.FirstName} {instructor.LastName}".Trim(),
            Email          = user?.Email    ?? instructor.Email,
            Department     = instructor.Department,
            Degree         = instructor.Degree,
            Phone          = instructor.Phone,
            IsActive       = instructor.IsActive,
            IsRegistered   = instructor.UserId.HasValue
        };
    }
}
