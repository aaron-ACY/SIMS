using SIMS.Application.DTOs.Users;
using SIMS.Application.Interfaces.Repositories;
using SIMS.Application.Interfaces.Services;
using SIMS.Domain.Constants;
using SIMS.Domain.Entities;
using SIMS.Shared.Exceptions;

namespace SIMS.Application.Services;

public class UserService : IUserService
{
    private const int MinUsernameLength = 6;
    private const int MinPasswordLength = 8;

    private readonly IUserRepository       _userRepository;
    private readonly IRoleRepository       _roleRepository;
    private readonly IPasswordHasher       _passwordHasher;
    private readonly IStudentRepository    _studentRepository;
    private readonly IInstructorRepository _instructorRepository;
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly IClassRepository      _classRepository;

    public UserService(
        IUserRepository       userRepository,
        IRoleRepository       roleRepository,
        IPasswordHasher       passwordHasher,
        IStudentRepository    studentRepository,
        IInstructorRepository instructorRepository,
        IEnrollmentRepository enrollmentRepository,
        IClassRepository      classRepository)
    {
        _userRepository       = userRepository;
        _roleRepository       = roleRepository;
        _passwordHasher       = passwordHasher;
        _studentRepository    = studentRepository;
        _instructorRepository = instructorRepository;
        _enrollmentRepository = enrollmentRepository;
        _classRepository      = classRepository;
    }

    public async Task<UserProfileResponse> GetMyProfileAsync(int userId, string roleName, CancellationToken ct = default)
    {
        var user = await _userRepository.GetByIdAsync(userId)
                   ?? throw new AppException(ErrorCode.USER_NOT_EXISTED);

        // Role is passed in from the JWT claim — no extra role DB round-trip needed.
        Student?    student    = null;
        Instructor? instructor = null;

        if (roleName == Roles.Student)
            student = await _studentRepository.GetByUserIdAsync(userId);
        else if (roleName == Roles.Instructor)
            instructor = await _instructorRepository.GetByUserIdAsync(userId);

        return MapToProfile(user, roleName, student, instructor);
    }

    public async Task<IEnumerable<UserProfileResponse>> GetAllAsync(CancellationToken ct = default)
    {
        var users = await _userRepository.GetAllAsync();
        var roles = await _roleRepository.GetAllAsync();

        // Lookup table: roleId → roleName
        var roleMap = roles.ToDictionary(r => r.Id, r => r.Name);

        return users.Select(u => MapToProfile(
            u,
            roleMap.TryGetValue(u.RoleId, out var name) ? name : string.Empty,
            student: null,
            instructor: null));
    }

    public async Task<UserProfileResponse> CreateStudentUserAsync(CreateStudentUserRequest request, CancellationToken ct = default)
    {
        var username = request.Username.Trim().ToLowerInvariant();
        var email    = request.Email.Trim().ToLowerInvariant();

        if (username.Length < MinUsernameLength)
            throw new AppException(ErrorCode.USERNAME_INVALID);

        if (request.Password.Length < MinPasswordLength)
            throw new AppException(ErrorCode.INVALID_PASSWORD);

        if (await _userRepository.GetByUsernameAsync(username) is not null)
            throw new AppException(ErrorCode.USER_EXISTED);

        if (await _userRepository.GetByEmailAsync(email) is not null)
            throw new AppException(ErrorCode.EMAIL_EXISTED);

        var role = await _roleRepository.GetByNameAsync(Roles.Student)
                   ?? throw new AppException(ErrorCode.ROLE_NOT_EXISTED);

        var (hash, salt) = _passwordHasher.HashPassword(request.Password);

        var user = new User
        {
            Username     = username,
            Email        = email,
            PasswordHash = hash,
            Salt         = salt,
            FirstName    = request.FirstName.Trim(),
            LastName     = request.LastName.Trim(),
            RoleId       = role.Id,
            IsActive     = true
        };
        await _userRepository.AddAsync(user);

        // Auto-generate a unique student code: BD + 5-digit zero-padded sequence.
        var allStudents = await _studentRepository.GetAllAsync();
        var studentCode = GenerateStudentCode(allStudents);

        var student = new Student
        {
            UserId         = user.Id,
            StudentCode    = studentCode,
            FirstName      = user.FirstName,
            LastName       = user.LastName,
            Email          = email,
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

        return MapToProfile(user, role.Name, student, instructor: null);
    }

    public async Task<UserProfileResponse> CreateInstructorUserAsync(CreateInstructorUserRequest request, CancellationToken ct = default)
    {
        var username = request.Username.Trim().ToLowerInvariant();
        var email    = request.Email.Trim().ToLowerInvariant();

        if (username.Length < MinUsernameLength)
            throw new AppException(ErrorCode.USERNAME_INVALID);

        if (request.Password.Length < MinPasswordLength)
            throw new AppException(ErrorCode.INVALID_PASSWORD);

        if (await _userRepository.GetByUsernameAsync(username) is not null)
            throw new AppException(ErrorCode.USER_EXISTED);

        if (await _userRepository.GetByEmailAsync(email) is not null)
            throw new AppException(ErrorCode.EMAIL_EXISTED);

        var role = await _roleRepository.GetByNameAsync(Roles.Instructor)
                   ?? throw new AppException(ErrorCode.ROLE_NOT_EXISTED);

        var (hash, salt) = _passwordHasher.HashPassword(request.Password);

        var user = new User
        {
            Username     = username,
            Email        = email,
            PasswordHash = hash,
            Salt         = salt,
            FirstName    = request.FirstName.Trim(),
            LastName     = request.LastName.Trim(),
            RoleId       = role.Id,
            IsActive     = true
        };
        await _userRepository.AddAsync(user);

        // Auto-generate a unique instructor code: GV + 5-digit zero-padded sequence.
        var allInstructors = await _instructorRepository.GetAllAsync();
        var instructorCode = GenerateInstructorCode(allInstructors);

        var instructor = new Instructor
        {
            UserId         = user.Id,
            InstructorCode = instructorCode,
            FirstName      = user.FirstName,
            LastName       = user.LastName,
            Email          = email,
            Department     = request.Department.Trim(),
            Degree         = request.Degree.Trim(),
            Phone          = request.Phone.Trim(),
            IsActive       = true
        };
        await _instructorRepository.AddAsync(instructor);

        return MapToProfile(user, role.Name, student: null, instructor);
    }

    public async Task<UserProfileResponse> CreateAsync(CreateUserRequest request, CancellationToken ct = default)
    {
        var username = request.Username.Trim().ToLowerInvariant();
        var email    = request.Email.Trim().ToLowerInvariant();

        if (username.Length < MinUsernameLength)
            throw new AppException(ErrorCode.USERNAME_INVALID);

        if (request.Password.Length < MinPasswordLength)
            throw new AppException(ErrorCode.INVALID_PASSWORD);

        if (await _userRepository.GetByUsernameAsync(username) is not null)
            throw new AppException(ErrorCode.USER_EXISTED);

        if (await _userRepository.GetByEmailAsync(email) is not null)
            throw new AppException(ErrorCode.EMAIL_EXISTED);

        var role = await _roleRepository.GetByNameAsync(request.RoleName.Trim())
                   ?? throw new AppException(ErrorCode.ROLE_NOT_EXISTED);

        var (hash, salt) = _passwordHasher.HashPassword(request.Password);

        var user = new User
        {
            Username     = username,
            Email        = email,
            PasswordHash = hash,
            Salt         = salt,
            FirstName    = request.FirstName.Trim(),
            LastName     = request.LastName.Trim(),
            RoleId       = role.Id,
            IsActive     = true
        };

        // AddAsync assigns Id, CreatedAt and UpdatedAt.
        await _userRepository.AddAsync(user);

        return MapToProfile(user, role.Name, student: null, instructor: null);
    }

    public async Task<UserProfileResponse> UpdateMyInfoAsync(int userId, string roleName, UpdateMyInfoRequest request, CancellationToken ct = default)
    {
        var user = await _userRepository.GetByIdAsync(userId)
                   ?? throw new AppException(ErrorCode.USER_NOT_EXISTED);

        // Apply only the fields that were explicitly provided (non-null).
        if (request.Email is not null)
        {
            var email = request.Email.Trim().ToLowerInvariant();

            // The email must stay unique across accounts — but the caller keeping
            // their own address unchanged is not a conflict.
            var existing = await _userRepository.GetByEmailAsync(email);
            if (existing is not null && existing.Id != userId)
                throw new AppException(ErrorCode.EMAIL_EXISTED);

            user.Email = email;
        }

        if (request.FirstName is not null)
            user.FirstName = request.FirstName.Trim();

        if (request.LastName is not null)
            user.LastName = request.LastName.Trim();

        // UpdateAsync refreshes UpdatedAt.
        await _userRepository.UpdateAsync(user);

        // Role is passed in from the JWT claim — no extra role DB round-trip needed.
        Student?    student    = null;
        Instructor? instructor = null;
        var phone = request.Phone?.Trim();

        if (roleName == Roles.Student)
        {
            student = await _studentRepository.GetByUserIdAsync(userId);
            if (student is not null)
            {
                if (request.Email     is not null) student.Email     = user.Email;
                if (request.FirstName is not null) student.FirstName = user.FirstName;
                if (request.LastName  is not null) student.LastName  = user.LastName;
                if (phone is { Length: > 0 })      student.Phone     = phone;

                await _studentRepository.UpdateAsync(student);
            }
        }
        else if (roleName == Roles.Instructor)
        {
            instructor = await _instructorRepository.GetByUserIdAsync(userId);
            if (instructor is not null)
            {
                if (request.Email     is not null) instructor.Email     = user.Email;
                if (request.FirstName is not null) instructor.FirstName = user.FirstName;
                if (request.LastName  is not null) instructor.LastName  = user.LastName;
                if (phone is { Length: > 0 })      instructor.Phone     = phone;

                await _instructorRepository.UpdateAsync(instructor);
            }
        }

        return MapToProfile(user, roleName, student, instructor);
    }

    public async Task DeleteAsync(int userId, int currentUserId, CancellationToken ct = default)
    {
        // Guard against an admin locking themselves out of the system.
        if (userId == currentUserId)
            throw new AppException(ErrorCode.CANNOT_DELETE_SELF);

        var user = await _userRepository.GetByIdAsync(userId)
                   ?? throw new AppException(ErrorCode.USER_NOT_EXISTED);

        var role = await _roleRepository.GetByIdAsync(user.RoleId);
        var roleName = role?.Name ?? string.Empty;

        // ── Check dependents + cascade-delete the profile record ─────── //
        if (roleName == Roles.Student)
        {
            var student = await _studentRepository.GetByUserIdAsync(userId);
            if (student is not null)
            {
                // Block deletion when the student is still enrolled in classes.
                if (await _enrollmentRepository.ExistsActiveForStudentAsync(student.Id))
                    throw new AppException(ErrorCode.USER_HAS_ACTIVE_ENROLLMENTS);

                await _studentRepository.DeleteAsync(student.Id);
            }
        }
        else if (roleName == Roles.Instructor)
        {
            var instructor = await _instructorRepository.GetByUserIdAsync(userId);
            if (instructor is not null)
            {
                // Block deletion when the instructor is still assigned to active classes.
                var activeClasses = (await _classRepository.GetAllAsync())
                    .Any(c => c.InstructorId == instructor.Id && c.IsActive);

                if (activeClasses)
                    throw new AppException(ErrorCode.USER_HAS_ACTIVE_CLASSES);

                await _instructorRepository.DeleteAsync(instructor.Id);
            }
        }

        await _userRepository.DeleteAsync(userId);
    }

    // ------------------------------------------------------------------ //

    // ── Code generation helpers ────────────────────────────────────────── //

    /// <summary>
    /// Returns the next available student code in BD00001 format.
    /// Parses the numeric suffix of all existing BD-prefixed codes and
    /// returns max + 1 zero-padded to 5 digits.
    /// </summary>
    private static string GenerateStudentCode(IEnumerable<Student> students)
    {
        var max = students
            .Select(s => s.StudentCode)
            .Where(c => c.StartsWith("BD", StringComparison.OrdinalIgnoreCase) && c.Length > 2)
            .Select(c => int.TryParse(c[2..], out var n) ? n : 0)
            .DefaultIfEmpty(0)
            .Max();
        return $"BD{max + 1:D5}";
    }

    /// <summary>
    /// Returns the next available instructor code in GV00001 format.
    /// Parses the numeric suffix of all existing GV-prefixed codes and
    /// returns max + 1 zero-padded to 5 digits.
    /// </summary>
    private static string GenerateInstructorCode(IEnumerable<Instructor> instructors)
    {
        var max = instructors
            .Select(i => i.InstructorCode)
            .Where(c => c.StartsWith("GV", StringComparison.OrdinalIgnoreCase) && c.Length > 2)
            .Select(c => int.TryParse(c[2..], out var n) ? n : 0)
            .DefaultIfEmpty(0)
            .Max();
        return $"GV{max + 1:D5}";
    }

    private static UserProfileResponse MapToProfile(
        Domain.Entities.User user,
        string roleName,
        Student?    student,
        Instructor? instructor) => new()
    {
        Username       = user.Username,
        Email          = user.Email,
        FirstName      = user.FirstName,
        LastName       = user.LastName,
        Role           = roleName,
        // Student-specific
        StudentCode    = student?.StudentCode,
        DateOfBirth    = student?.DateOfBirth,
        Gender         = student?.Gender,
        Major          = student?.Major,
        // Instructor-specific
        InstructorCode = instructor?.InstructorCode,
        Department     = instructor?.Department,
        Degree         = instructor?.Degree,
        // Shared optional
        Phone          = student?.Phone ?? instructor?.Phone,
        Address        = student?.Address
    };
}
