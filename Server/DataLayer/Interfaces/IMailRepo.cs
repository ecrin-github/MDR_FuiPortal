namespace MDR_FuiPortal.Server;

public interface IMailRepo
{
    Task SendEmailAsync(string ToEmail, string Subject, string Body);
}