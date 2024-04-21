using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MyFreeFormForm.Helpers;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace MyFreeFormForm.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly ILogger<EmailSender> _logger;
        private readonly SendGridClient _sendGridClient;
        private readonly EmailSenderOptions _options;

        public EmailSender(IOptions<EmailSenderOptions> optionsAccessor, ILogger<EmailSender> logger)
        {
            _logger = logger;
            _options = optionsAccessor.Value;

            var apiKey = EncryptionHelper.Decrypt(_options.ApiKey);
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new ArgumentException("SendGrid API key is missing");
            }

            _sendGridClient = new SendGridClient(apiKey);
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var from = new EmailAddress(EncryptionHelper.Decrypt(_options.FromEmail), _options.FromName);
            var to = new EmailAddress(email);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, null, htmlMessage);
           
           
            try
            {
                var response = await _sendGridClient.SendEmailAsync(msg);
                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation($"Email to {email} queued successfully!");
                }
                else
                {
                    _logger.LogError($"Failed to send email to {email}: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send email to {email}");
                throw;
            }
        }
    }

    public class EmailSenderOptions
    {
        public string ApiKey { get; set; }
        public string FromEmail { get; set; }
        public string FromName { get; set; }
    }
}
