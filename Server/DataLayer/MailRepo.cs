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
        MailMessage message = new MailMessage();
        SmtpClient smtp = new SmtpClient();
        message.From = new MailAddress(_mailConfig.FromEmail);
        message.To.Add(new MailAddress(ToEmail));
        message.Subject = Subject;
        message.IsBodyHtml = true;
        message.Body = Body;
        smtp.Port = _mailConfig.Port;
        smtp.Host = _mailConfig.Host;
        smtp.EnableSsl = true;
        smtp.UseDefaultCredentials = false;
        smtp.Credentials = new NetworkCredential(_mailConfig.UserName, _mailConfig.Password);
        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
        await smtp.SendMailAsync(message);
    }
}
