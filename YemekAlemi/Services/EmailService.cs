using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using YemekAlemi.Data;
using YemekAlemi.Models;

namespace YemekAlemi.Services
{
    public class EmailService
    {
        private readonly EmailSettings _settings;
        private readonly AppDbContext _context;

        public EmailService(
            IOptions<EmailSettings> settings,
            AppDbContext context)
        {
            _settings = settings.Value;
            _context = context;
        }

        public void SendEmail(
            string toEmail,
            string subject,
            string body,
            string? userId = null)
        {
            var message = new MailMessage();

            message.From =
                new MailAddress(
                    _settings.SenderEmail,
                    _settings.SenderName);

            message.To.Add(toEmail);

            message.Subject = subject;
            message.Body = body;

            using var smtp = new SmtpClient(
                _settings.SmtpServer,
                _settings.Port);

            smtp.Credentials =
                new NetworkCredential(
                    _settings.Username,
                    _settings.Password);

            smtp.EnableSsl = true;

            smtp.Send(message);

            _context.EmailLogs.Add(new EmailLog
            {
                UserId = userId,
                ToEmail = toEmail,
                Subject = subject,
                Body = body,
                SentAt = DateTime.Now
            });

            _context.SaveChanges();
        }
    }
}