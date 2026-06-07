using E_Commerce.Application.Contracts.Infrastructure.Emails;
using E_Commerce.Domain.Common.Errors;
using E_Commerce.Domain.ValueObjects;
using E_Commerce.Infrastructure.Exceptions;
using E_Commerce.Infrastructure.Settings;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using Polly;
using Polly.Registry;

internal sealed class EmailSender : IEmailSender
{
    private readonly MailTrapOptions _options;
    private readonly ResiliencePipeline _pipeline;
    private readonly ILogger<EmailSender> _logger;

    public EmailSender(
        IOptions<MailTrapOptions> options,
        ResiliencePipelineProvider<string> pipeline,
        ILogger<EmailSender> logger)
    {
        _options = options.Value;
        _pipeline = pipeline.GetPipeline("emailJob");
        _logger = logger;
    }

    public async Task SendAsync(
        EmailAddress recipient,
        string subject,
        string htmlBody,
        CancellationToken cancellationToken)
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

        _logger.LogInformation("Email provider send started");

        try
        {
            await _pipeline.ExecuteAsync(async ctx =>
            {
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
                    throw new InfrastructureException(InfrastructureErrors.PersistenceFailure);
                }
            }, cancellationToken);

            _logger.LogInformation("Email provider send completed");
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Email provider send failed");

            throw;
        }
    }
}
