using Microsoft.AspNetCore.Mvc;

namespace MDR_FuiPortal.Server.Controllers;

[Route("api/email/[controller]")]
public class EmailController : ControllerBase
{
    private readonly IMailRepo _mailRepo;

    public EmailController(IMailRepo mailRepo)
    {
        _mailRepo = mailRepo;
    }

    [HttpPost("send-email")]
    public async Task<IActionResult> SendEmailAsync([FromBody] EmailBody message)
    {
        try
        {
            await _mailRepo.SendEmailAsync(message.toEmail, message.subject, message.message);
            return Ok("email sent");
        }
        catch (Exception ex)
        {
            return BadRequest(@$"error when sending an email: {ex.Message}");
        }
    }
}

public record EmailBody(
    string toEmail,
    string subject,
    string message
);