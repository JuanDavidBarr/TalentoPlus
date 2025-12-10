using TalentoPlus.Models;

namespace TalentoPlus.Repositories.Interfaces
{
    public interface IEmployeeRepository
    {
        Task<IEnumerable<Employee>> GetAllAsync();
        Task<Employee> GetByIdAsync(int id);
        Task<Employee> CreateAsync(Employee employee);
        Task<Employee> UpdateAsync(Employee employee);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<bool> DocumentNumberExistsAsync(string documentNumber, int? excludeId = null);
        Task<bool> EmailExistsAsync(string email, int? excludeId = null);
    }
}