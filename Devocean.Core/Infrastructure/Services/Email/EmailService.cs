using MimeKit.Text;

namespace Devocean.Core.Infrastructure.Services.Email;

public class EmailService
{
    private readonly EmailMessage _emailMessage;

    public EmailService(EmailMessage emailMessage)
    {
        _emailMessage = emailMessage ?? throw new ArgumentNullException(nameof(emailMessage));
    }

    public async ValueTask<string?> SendHtmlMessage(string fromEmail, string toEmail, string subject, string content,
        CancellationToken cancellationToken = default)
    {
        _emailMessage.Create(fromEmail, toEmail, subject, TextFormat.Html, content);
        return await _emailMessage.SendAsync(cancellationToken: cancellationToken);
    }

    public async ValueTask<string?> SendPlainTextMessage(string fromEmail, string toEmail, string subject,
        string content, CancellationToken cancellationToken = default)
    {
        _emailMessage.Create(fromEmail, toEmail, subject, TextFormat.Plain, content);
        return await _emailMessage.SendAsync(cancellationToken: cancellationToken);
    }
}