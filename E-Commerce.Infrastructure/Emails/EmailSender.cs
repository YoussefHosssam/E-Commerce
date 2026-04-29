using E_Commerce.Application.Contracts.Infrastructure.Emails;
using E_Commerce.Domain.Common.Errors;
using E_Commerce.Domain.ValueObjects;
using E_Commerce.Infrastructure.Exceptions;
using E_Commerce.Infrastructure.Settings;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

internal sealed class EmailSender : IEmailSender
{
    private readonly MailTrapProviderOptions _options;

    public EmailSender(IOptions<MailTrapProviderOptions> options)
    {
        _options = options.Value;
    }

    public async Task SendAsync(
        EmailAddress recipient,
        string subject,
        string htmlBody,
        CancellationToken cancellationToken = default)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("App", _options.Sender));
        message.To.Add(MailboxAddress.Parse(recipient.Value));
        message.Subject = subject;

        message.Body = new BodyBuilder
        {
            HtmlBody = htmlBody,
            TextBody = "Please view this email in an HTML-compatible client."
        }.ToMessageBody();

        using var smtp = new SmtpClient();

        try
        {
            await smtp.ConnectAsync(
                _options.Host,
                _options.Port,
                SecureSocketOptions.Auto,
                cancellationToken);
            await smtp.AuthenticateAsync(
                _options.Username,
                _options.Password,
                cancellationToken);
            await smtp.SendAsync(message, cancellationToken);
            await smtp.DisconnectAsync(true, cancellationToken);
        }
        catch (Exception)
        {
            throw new InfrastructureException(ErrorCodes.Infrastructure.PersistenceFailure);
        }
    }
}