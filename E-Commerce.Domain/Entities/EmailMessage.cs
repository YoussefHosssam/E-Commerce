// Domain/Entities/EmailMessage.cs

using E_Commerce.Domain.Common;
using E_Commerce.Domain.Common.Errors;
using E_Commerce.Domain.Enums;
using E_Commerce.Domain.ValueObjects;

namespace E_Commerce.Domain.Entities
{
    public class EmailMessage : BaseEntity
    {
        public Guid? UserId { get; private set; }
        public Guid? RelatedTokenId { get; private set; }

        public MessageType MessageType { get; private set; } = default!;
        public string Subject { get; private set; } = default!;
        public string BodyHtml { get; private set; } = default!;
        public string ProviderName { get; private set; } = "MailTrap";
        public EmailAddress Sender { get; private set; } = EmailAddress.Create("info@gmail.com");
        public EmailAddress Recipient { get; private set; } = default!;
        public EmailStatus Status { get; private set; } = EmailStatus.Pending;
        public int Attempts { get; private set; } = 0;
        public DateTimeOffset? SentAt { get; private set; }
        public DateTimeOffset? LastAttemptAt { get; private set; }
        public string? LastError { get; private set; }

        public User? User { get; private set; }
        public AuthToken? RelatedToken { get; private set; }

        // EF Core
        private EmailMessage() { }

        private EmailMessage(
            Guid? userId,
            Guid? relatedTokenId,
            MessageType messageType,
            EmailAddress recipient,
            string subject,
            string bodyHtml)
        {
            UserId = userId;
            RelatedTokenId = relatedTokenId;
            MessageType = messageType;
            Subject = subject;
            BodyHtml = bodyHtml;
            Recipient = recipient;
            Status = EmailStatus.Pending;
            Attempts = 0;
        }

        public static EmailMessage Create(
            Guid? userId,
            Guid? relatedTokenId,
            MessageType messageType,
            EmailAddress recipient,
            string subject,
            string bodyHtml)
        {
            if (!Enum.IsDefined(typeof(MessageType), messageType))
                throw new DomainValidationException(ErrorCodes.Domain.EmailMessage.TypeInvalid);

            subject = NormalizeRequired(subject, 300, ErrorCodes.Domain.EmailMessage.SubjectRequired, ErrorCodes.Domain.EmailMessage.SubjectTooLong, "Subject");
            bodyHtml = NormalizeRequired(bodyHtml, 20000, ErrorCodes.Domain.EmailMessage.BodyRequired, ErrorCodes.Domain.EmailMessage.BodyTooLong, "BodyHtml");

            return new EmailMessage(userId, relatedTokenId, messageType,recipient , subject, bodyHtml);
        }

        public void MarkAsProcessing(string providerName)
        {
            providerName = NormalizeOptional(providerName, 100, ErrorCodes.Domain.EmailMessage.ProviderTooLong, "ProviderName")
                           ?? throw new DomainValidationException(ErrorCodes.Domain.EmailMessage.ProviderRequired);

            ProviderName = providerName;
            Status = EmailStatus.Processing;
            Attempts++;
            LastAttemptAt = DateTimeOffset.UtcNow;
            LastError = null;
        }

        public void MarkAsSent(string providerName)
        {
            providerName = NormalizeOptional(providerName, 100, ErrorCodes.Domain.EmailMessage.ProviderTooLong, "ProviderName")
                           ?? throw new DomainValidationException(ErrorCodes.Domain.EmailMessage.ProviderRequired);

            ProviderName = providerName;
            Status = EmailStatus.Sent;
            SentAt = DateTimeOffset.UtcNow;
            LastAttemptAt = DateTimeOffset.UtcNow;
            LastError = null;
        }
        public void MarkAttempt()
        {
            Attempts++;
            LastAttemptAt = DateTimeOffset.UtcNow;
        }

        public void MarkAsFailed(string error)
        {
            error = NormalizeRequired(error, 4000, ErrorCodes.Domain.EmailMessage.ErrorRequired, ErrorCodes.Domain.EmailMessage.ErrorTooLong, "LastError");

            Status = EmailStatus.Failed;
            LastAttemptAt = DateTimeOffset.UtcNow;
            LastError = error;
        }

        public void ResetToPending()
        {
            Status = EmailStatus.Pending;
            LastError = null;
        }

        private static string NormalizeRequired(
            string value,
            int maxLength,
            string requiredCode,
            string tooLongCode,
            string fieldName)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainValidationException(requiredCode);

            value = value.Trim();

            if (value.Length > maxLength)
                throw new DomainValidationException(tooLongCode);

            return value;
        }

        private static string? NormalizeOptional(
            string? value,
            int maxLength,
            string tooLongCode,
            string fieldName)
        {
            if (value is null)
                return null;

            value = value.Trim();

            if (value.Length == 0)
                return null;

            if (value.Length > maxLength)
                throw new DomainValidationException(tooLongCode);

            return value;
        }
        public void SetSenderEmail(EmailAddress sender)
        {
            Sender = sender;
        }
    }
}

