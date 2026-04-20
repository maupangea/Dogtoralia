namespace DogtoraliaMVC.Services;

public interface IEmailService
{
    Task SendWelcomeEmailAsync(string toEmail, string toName);
}
