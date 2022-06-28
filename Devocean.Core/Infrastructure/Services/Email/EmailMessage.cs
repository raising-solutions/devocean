using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;

namespace Devocean.Core.Infrastructure.Services.Email;

public class EmailMessage
{
    private readonly SmtpConfig _smtpConfig;
    public MimeMessage Email { get; private set; }

    public EmailMessage(SmtpConfig smtpConfig)
    {
        _smtpConfig = smtpConfig ?? throw new ArgumentNullException(nameof(smtpConfig));
        Email = new MimeMessage();
    }

    public void Create(string fromEmail, string toEmail, string subject, TextFormat format, string content)
    {
        Email = new MimeMessage();
        Email.From.Add(MailboxAddress.Parse(fromEmail));
        Email.To.Add(MailboxAddress.Parse(toEmail));
        Email.Subject = subject;
        Email.Body = new TextPart(format) { Text = content };
    }

    public async ValueTask<string?> SendAsync(MimeMessage? email = null, CancellationToken cancellationToken = default)
    {
        using var smtpClient = new SmtpClient();
        await smtpClient.ConnectAsync(_smtpConfig.Endpoint, _smtpConfig.Port, _smtpConfig.SecureSocketOptions,
            cancellationToken);
        if (!string.IsNullOrWhiteSpace(_smtpConfig.UserName) || !string.IsNullOrWhiteSpace(_smtpConfig.Password))
        {
            await smtpClient.AuthenticateAsync(_smtpConfig.UserName, _smtpConfig.Password, cancellationToken);
        }

        var serverResponse = await smtpClient.SendAsync(email ?? Email, cancellationToken);
        await smtpClient.DisconnectAsync(true, cancellationToken);
        return serverResponse;
    }
}