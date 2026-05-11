using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using YemekAlemi.Data;

namespace YemekAlemi.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            if (User.IsInRole("Admin"))
            {
                ViewBag.TotalUsers = _context.Users.Count();
                ViewBag.TotalOrders = _context.Orders.Count();
                ViewBag.TotalRevenue = _context.Orders.Any()
                    ? _context.Orders.Sum(x => x.TotalPrice)
                    : 0;
                ViewBag.TotalRatings = _context.Ratings.Count();

                return View("AdminDashboard");
            }

            if (User.IsInRole("Caretaker"))
            {
                ViewBag.TotalMenus = _context.Foods.Count();
                ViewBag.TotalRatings = _context.Ratings.Count();
                ViewBag.AverageRating = _context.Ratings.Any()
                    ? Math.Round(_context.Ratings.Average(x => x.CatererRating), 1)
                    : 0;

                return View("CatererDashboard");
            }

            if (User.IsInRole("User"))
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var recentLogs = _context.AppLogs
    .Where(x => x.UserId == userId)
    .OrderByDescending(x => x.CreatedAt)
    .Take(5)
    .ToList();

                ViewBag.RecentLogs = recentLogs;

                ViewBag.TotalOrders = _context.Orders.Count(x => x.UserId == userId);
                ViewBag.CompletedOrders = _context.Orders.Count(x => x.UserId == userId && x.Status == "Completed");
                ViewBag.TotalSpent = _context.Orders.Any(x => x.UserId == userId)
                    ? _context.Orders.Where(x => x.UserId == userId).Sum(x => x.TotalPrice)
                    : 0;

                return View("UserDashboard");
            }

            return RedirectToAction("Index", "Home");
        }
    }
}