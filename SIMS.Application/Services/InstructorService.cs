using SIMS.Application.DTOs.Instructors;
using SIMS.Application.Interfaces.Repositories;
using SIMS.Application.Interfaces.Services;

namespace SIMS.Application.Services;

public class InstructorService : IInstructorService
{
    private readonly IInstructorRepository _instructorRepository;
    private readonly IUserRepository _userRepository;

    public InstructorService(
        IInstructorRepository instructorRepository,
        IUserRepository userRepository)
    {
        _instructorRepository = instructorRepository;
        _userRepository = userRepository;
    }

    public async Task<IEnumerable<InstructorResponse>> GetAllAsync()
    {
        var instructors = await _instructorRepository.GetAllAsync();

        // One read of users.csv, then an in-memory join — avoids a lookup per instructor.
        var users = await _userRepository.GetAllAsync();
        var userMap = users.ToDictionary(u => u.Id);

        return instructors.Select(i =>
        {
            userMap.TryGetValue(i.UserId, out var user);

            return new InstructorResponse
            {
                InstructorCode = i.InstructorCode,
                FullName       = user?.FullName ?? string.Empty,
                Email          = user?.Email ?? string.Empty,
                Department     = i.Department,
                Degree         = i.Degree,
                Specialization = i.Specialization,
                HireDate       = i.HireDate,
                Phone          = i.Phone,
                IsActive       = i.IsActive
            };
        });
    }
}
