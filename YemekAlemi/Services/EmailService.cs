using YemekAlemi.Data;
using YemekAlemi.Models;

namespace YemekAlemi.Services
{
    public class EmailService
    {
        private readonly AppDbContext _context;
        private readonly LogService _logService;

        public EmailService(AppDbContext context, LogService logService)
        {
            _context = context;
            _logService = logService;
        }

        public void SendEmail(string toEmail, string subject, string body, string? userId = null)
        {
            var email = new EmailLog
            {
                ToEmail = toEmail,
                Subject = subject,
                Body = body,
                SentAt = DateTime.Now
            };

            _context.EmailLogs.Add(email);
            _context.SaveChanges();

            _logService.AddLog(
                "Email",
                $"Email sent to {toEmail}. Subject: {subject}",
                userId,
                toEmail
            );
        }
    }
}