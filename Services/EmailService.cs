using System.Net;
using System.Net.Mail;

namespace CarRentalAPI.Services
{
    public class EmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public void SendEmail(
            string toEmail,
            string subject,
            string body)
        {
            var fromEmail =
                _config["EmailSettings:Email"];

            var password =
                _config["EmailSettings:Password"];

            var host =
                _config["EmailSettings:Host"];

            var port = int.Parse(
                _config["EmailSettings:Port"]);

            var smtp = new SmtpClient(host)
            {
                Port = port,
                Credentials =
                    new NetworkCredential(
                        fromEmail,
                        password),
                EnableSsl = true
            };

            var message = new MailMessage(
                fromEmail,
                toEmail,
                subject,
                body
            );

            smtp.Send(message);
        }
    }
}