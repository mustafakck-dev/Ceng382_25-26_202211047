using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YemekAlemi.Data;

namespace YemekAlemi.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminLogsController : Controller
    {
        private readonly AppDbContext _context;

        public AdminLogsController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string? search, string? eventType, int page = 1)
        {
            int pageSize = 5;

            var logs = _context.AppLogs.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                logs = logs.Where(x =>
                    x.Message.Contains(search) ||
                    (x.UserEmail != null && x.UserEmail.Contains(search)));
            }

            if (!string.IsNullOrEmpty(eventType))
            {
                logs = logs.Where(x => x.EventType == eventType);
            }

            int totalLogs = logs.Count();

            var pagedLogs = logs
                .OrderByDescending(x => x.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.Search = search;
            ViewBag.EventType = eventType;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalLogs / (double)pageSize);

            return View(pagedLogs);
        }
    }
}