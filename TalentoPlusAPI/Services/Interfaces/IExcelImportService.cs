namespace TalentoPlus.Services.Interfaces
{
    public interface IExcelImportService
    {
        Task<int> ImportEmployeesFromExcelAsync(string filePath);
    }
}