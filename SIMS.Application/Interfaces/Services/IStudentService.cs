using SIMS.Application.DTOs.Students;

namespace SIMS.Application.Interfaces.Services;

public interface IStudentService
{
    Task<IEnumerable<StudentResponse>> GetAllAsync();
    Task<StudentResponse> GetByIdAsync(int id);
    Task<StudentResponse> CreateAsync(CreateStudentRequest request);
    Task<StudentResponse> UpdateAsync(int id, UpdateStudentRequest request);
    Task DeleteAsync(int id);
}
