using MDR_FuiPortal.Shared;
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;

namespace MDR_FuiPortal.Server;

public class MailRepo : IMailRepo
{
    private readonly MailConfigModel _mailConfig;
    public MailRepo(IOptions<MailConfigModel> mailConfig)
    {
        _mailConfig = mailConfig.Value;
    }

    public async Task SendEmailAsync(string ToEmail, string Subject, string Body)
    {
        var client = new SmtpClient(_mailConfig.Host)
        {
            Port = _mailConfig.Port,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false,
            EnableSsl = true,
            Credentials = new NetworkCredential(_mailConfig.UserName, _mailConfig.Password)
        };

        var message = new MailMessage()
        {
            From = new MailAddress(_mailConfig.FromEmail),
            Subject = Subject,
            IsBodyHtml = false,
            Body = Body
        };
        message.To.Add(new MailAddress(ToEmail));
        
        await client.SendMailAsync(message);
    }
}
