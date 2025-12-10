namespace TalentoPlus.Services.Interfaces
{
    public interface IResumeService
    {
        Task<byte[]> GenerateEmployeeResumeAsync(int employeeId);
    }
}