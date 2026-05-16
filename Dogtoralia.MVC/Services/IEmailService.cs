namespace Dogtoralia.MVC.Services;

public interface IEmailService
{
    Task SendWelcomeEmailAsync(string toEmail, string toName);
}
