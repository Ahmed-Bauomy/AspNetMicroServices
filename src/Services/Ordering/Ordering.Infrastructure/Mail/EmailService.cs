using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Ordering.Application.Contracts.Infrastructure;
using Ordering.Application.Models;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Infrastructure.Mail
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings emailSettings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<EmailSettings> options, ILogger<EmailService> logger)
        {
            emailSettings = options.Value ?? throw new ArgumentNullException(nameof(EmailSettings));
            _logger = logger;
        }
        public async Task<bool> SendEmail(Email email)
        {
            var client = new SendGridClient(emailSettings.ApiKey);
            var fromAddress = emailSettings.FromAddress;
            var fromName = emailSettings.FromName;

            var from = new EmailAddress()
            {
                Email = fromAddress,
                Name = fromName
            };
            var to = new EmailAddress(email.To);
            var subject = email.Subject;
            var emailBody = email.Body;

            var emailMessage = MailHelper.CreateSingleEmail(from,to,subject, emailBody, emailBody);
            var response = await client.SendEmailAsync(emailMessage);
            _logger.LogInformation("Emial sent.");
            if (response.StatusCode == HttpStatusCode.Accepted || response.StatusCode == HttpStatusCode.OK)
            {
                return true;
            }
            _logger.LogError("Email sending failed.");
            return false;
        }
    }
}
