using MailKit.Net.Smtp;
using MimeKit;

public class EmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse(_configuration["SmtpSettings:Username"]));
        email.To.Add(MailboxAddress.Parse(to));
        email.Subject = subject;
        email.Body = new TextPart("html") { Text = body };

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(
            _configuration["SmtpSettings:Host"],
            int.Parse(_configuration["SmtpSettings:Port"]),
            MailKit.Security.SecureSocketOptions.StartTls
        );
        await smtp.AuthenticateAsync(
            _configuration["SmtpSettings:Username"],
            _configuration["SmtpSettings:Password"]
        );
        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);

        Console.WriteLine($"Email sent to {to}: {subject}");
    }
}
