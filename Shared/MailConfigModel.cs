namespace MDR_FuiPortal.Shared;

public class MailConfigModel
{
    public const string SectionName = "SmtpConfig";
    
    public string UserName {get; set; }
    public string Password {get; set; }
    public int Port {get; set; }
    public string FromEmail {get; set; }
    public string Host {get; set; }
    public string DisplayName { get; set; }
}