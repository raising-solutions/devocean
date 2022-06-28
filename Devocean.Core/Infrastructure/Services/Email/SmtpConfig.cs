using MailKit.Security;

namespace Devocean.Core.Infrastructure.Services.Email;

public class SmtpConfig
{
    public SmtpConfig(string endpoint, int port, string userName, string password,
        SecureSocketOptions secureSocketOptions)
    {
        Endpoint = endpoint ?? throw new ArgumentNullException(nameof(endpoint));
        Port = port;
        UserName = userName ?? throw new ArgumentNullException(nameof(userName));
        Password = password ?? throw new ArgumentNullException(nameof(password));
        SecureSocketOptions = secureSocketOptions;
    }

    public string Endpoint { get; private set; }
    public string UserName { get; private set; }
    public string Password { get; private set; }
    public int Port { get; private set; }
    public SecureSocketOptions SecureSocketOptions { get; private set; }
}