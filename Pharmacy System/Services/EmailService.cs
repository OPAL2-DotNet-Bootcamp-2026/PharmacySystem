using System.Net;
using System.Net.Mail;

namespace Pharmacy_System.Services
{
    public class EmailService
    {
        private readonly IConfiguration config;
        private readonly ILogger<EmailService> logger;

        public EmailService(
            IConfiguration config,
            ILogger<EmailService> logger)
        {
            this.config = config;
            this.logger = logger;
        }

        public async Task SendAsync(
            string toEmail,
            string subject,
            string body)
        {
            // Let the demo run even if SMTP is blocked
            bool enabled =
                config["EmailSettings:Enabled"] == "true";

            if (!enabled)
            {
                logger.LogInformation(
                    "Email DISABLED. Would have sent to {To}: {Subject}",
                    toEmail,
                    subject);

                return;
            }

            try
            {
                string senderEmail =
                    config["EmailSettings:SenderEmail"]!;

                string senderName =
                    config["EmailSettings:SenderName"]!;

                string password =
                    config["EmailSettings:Password"]!;

                string host =
                    config["EmailSettings:Host"]!;

                int port =
                    int.Parse(
                        config["EmailSettings:Port"]!);

                using SmtpClient client =
                    new SmtpClient(host, port)
                    {
                        EnableSsl = true,
                        Credentials =
                            new NetworkCredential(
                                senderEmail,
                                password)
                    };

                using MailMessage message =
                    new MailMessage
                    {
                        From = new MailAddress(
                            senderEmail,
                            senderName),

                        Subject = subject,
                        Body = body,
                        IsBodyHtml = false
                    };

                message.To.Add(toEmail);

                await client.SendMailAsync(message);

                logger.LogInformation(
                    "Email sent to {To}: {Subject}",
                    toEmail,
                    subject);
            }
            catch (Exception ex)
            {
                // Email failure must not cancel approval
                logger.LogError(
                    ex,
                    "Failed to send email to {To}",
                    toEmail);
            }
        }
    }
}