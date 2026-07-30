using SIMS.Application.DTOs.Instructors;

namespace SIMS.Application.Interfaces.Services;

public interface IInstructorService
{
    /// <summary>
    /// Returns every instructor with name and email resolved from the linked user.
    /// Requires the VIEW_INSTRUCTORS permission.
    /// </summary>
    Task<IEnumerable<InstructorResponse>> GetAllAsync();
}
