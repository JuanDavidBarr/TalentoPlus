namespace TalentoPlus.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendWelcomeEmailAsync(string toEmail, string employeeName);
    }
}