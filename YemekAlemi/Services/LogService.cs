using YemekAlemi.Data;
using YemekAlemi.Models;

namespace YemekAlemi.Services
{
    public class LogService
    {
        private readonly AppDbContext _context;

        public LogService(AppDbContext context)
        {
            _context = context;
        }

        public void AddLog(string eventType, string message, string? userId = null, string? userEmail = null)
        {
            var log = new AppLog
            {
                EventType = eventType,
                Message = message,
                UserId = userId,
                UserEmail = userEmail,
                CreatedAt = DateTime.Now
            };

            _context.AppLogs.Add(log);
            _context.SaveChanges();
        }
    }
}