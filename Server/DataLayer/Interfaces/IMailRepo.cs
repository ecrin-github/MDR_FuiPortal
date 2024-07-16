using MDR_FuiPortal.Shared;
namespace MDR_FuiPortal.Server;

public interface IMailRepo
{
    public Task SendEmailAsync(string ToEmail, string Subject, string Body);
}