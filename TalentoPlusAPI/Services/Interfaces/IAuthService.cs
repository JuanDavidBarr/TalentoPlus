using TalentoPlus.DTOs;
using TalentoPlus.DTOs.Auth;

namespace TalentoPlus.Services.Interfaces
{
    public interface IAuthService
    {
        Task<List<DepartmentDto>> GetDepartmentsAsync();
        Task<EmployeeDto> SelfRegisterAsync(SelfRegisterDto dto);
        Task<LoginResponseDto> LoginAsync(LoginDto dto);
        Task<EmployeeDto> GetMyInfoAsync(int employeeId);
    }
}