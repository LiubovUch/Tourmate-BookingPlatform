using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Threading.Tasks;

namespace Assignment1.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly string _sendGridKey;
        private readonly ILogger<EmailSender> _logger;

        public EmailSender(IConfiguration configuration, ILogger<EmailSender> logger)
        {
            _sendGridKey = configuration["SendGrid:ApiKey"];
            _logger = logger;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            try
            {
                var client = new SendGridClient(_sendGridKey);
                var from = new EmailAddress("liuchamprina@gmail.com", "Project Collaborate");
                var to = new EmailAddress(email);
                var msg = MailHelper.CreateSingleEmail(from, to, subject, "", htmlMessage);
                await client.SendEmailAsync(msg);
                _logger.LogInformation("Email sent successfully to {Email} with subject {Subject}", email, subject);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email to {Email} with subject {Subject}", email, subject);
                throw; // Re-throw the exception to propagate it up the call stack
            }
        }
    }
}

