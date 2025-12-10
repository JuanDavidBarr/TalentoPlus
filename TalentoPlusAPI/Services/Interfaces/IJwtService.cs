using TalentoPlus.Models;

namespace TalentoPlus.Services.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(Employee employee);
    }
}