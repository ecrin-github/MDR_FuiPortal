using MDR_FuiPortal.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;

namespace MDR_FuiPortal.Server;

public class  MailRepo: IMailRepo
{
    private readonly MailSettings _mailConfig;
    public MailRepo(MailSettings mailConfig)
    {
        _mailConfig = mailConfig;
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
