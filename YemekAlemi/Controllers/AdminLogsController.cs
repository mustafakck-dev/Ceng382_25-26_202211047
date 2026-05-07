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

        public IActionResult Index()
        {
            var logs = _context.AppLogs
                .OrderByDescending(x => x.CreatedAt)
                .ToList();

            return View(logs);
        }
    }
}