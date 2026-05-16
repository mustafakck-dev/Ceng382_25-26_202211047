using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YemekAlemi.Data;

namespace YemekAlemi.Controllers
{
    [Authorize]
    public class NotificationsController : Controller
    {
        private readonly AppDbContext _context;

        public NotificationsController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var userEmail = User.Identity?.Name;

            var notifications = _context.AppLogs
                .Where(x =>
                    x.UserEmail == userEmail ||
                    User.IsInRole("Admin"))
                .OrderByDescending(x => x.CreatedAt)
                .Take(20)
                .ToList();

            return View(notifications);
        }
    }
}