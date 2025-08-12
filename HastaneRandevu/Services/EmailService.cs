using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace HastaneRandevu.Services
{
    public class EmailService : IEmailService, IEmailSender
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlMessage)
        {
            var smtpSettings = _configuration.GetSection("SmtpSettings");
            var host = smtpSettings["Host"];
            var port = int.Parse(smtpSettings["Port"]);
            var user = smtpSettings["UserName"];
            var pass = smtpSettings["Password"];
            var enableSsl = bool.Parse(smtpSettings["EnableSSL"]);

            using var client = new SmtpClient(host, port)
            {
                Credentials = new NetworkCredential(user, pass),
                EnableSsl = enableSsl
            };

            var mail = new MailMessage(user, toEmail, subject, htmlMessage)
            {
                IsBodyHtml = true
            };

            await client.SendMailAsync(mail);
        }

        // IEmailSender implementation for Identity
        async Task IEmailSender.SendEmailAsync(string email, string subject, string htmlMessage)
        {
            await SendEmailAsync(email, subject, htmlMessage);
        }
    }
}
