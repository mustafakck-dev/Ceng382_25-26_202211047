using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
                ViewBag.TotalLogs = _context.AppLogs.Count();
                ViewBag.TotalEmails = _context.EmailLogs.Count();

                ViewBag.RecentLogs = _context.AppLogs
                    .OrderByDescending(x => x.CreatedAt)
                    .Take(5)
                    .ToList();

                ViewBag.CompletedOrders = _context.Orders.Count(x => x.Status == "Completed");
                ViewBag.PreparingOrders = _context.Orders.Count(x => x.Status == "Preparing");
                ViewBag.ReadyOrders = _context.Orders.Count(x => x.Status == "Ready");
                ViewBag.DeliveredOrders = _context.Orders.Count(x => x.Status == "Delivered");

                ViewBag.TopPackages = _context.OrderItems
                    .GroupBy(x => x.Name)
                    .Select(g => new
                    {
                        Name = g.Key,
                        Count = g.Count()
                    })
                    .OrderByDescending(x => x.Count)
                    .Take(5)
                    .ToList();
                return View("AdminDashboard");
            }

            if (User.IsInRole("Caterer"))
            {
                var catererEmail = User.Identity?.Name;

                var catererFoodIds = _context.Foods
                    .Where(f => f.CatererEmail == catererEmail)
                    .Select(f => f.Id)
                    .ToList();

                var catererOrders = _context.Orders
                    .Include(o => o.Items)
                    .Where(o => o.Items.Any(i => catererFoodIds.Contains(i.FoodId)));

                ViewBag.TotalPackages = _context.Foods
                    .Count(x => x.CatererEmail == catererEmail);

                ViewBag.TotalRatings = _context.Ratings.Count();

                ViewBag.AverageRating = _context.Ratings.Any()
                    ? Math.Round(_context.Ratings.Average(x => x.CatererRating), 1)
                    : 0;

                ViewBag.ActiveOrders = catererOrders
                    .Count(o => o.Status == "Completed" || o.Status == "Preparing" || o.Status == "Ready");

                ViewBag.MonthlyRevenue = catererOrders
                    .Where(o => o.CreatedAt.Month == DateTime.Now.Month &&
                                o.CreatedAt.Year == DateTime.Now.Year)
                    .Any()
                        ? catererOrders
                            .Where(o => o.CreatedAt.Month == DateTime.Now.Month &&
                                        o.CreatedAt.Year == DateTime.Now.Year)
                            .Sum(o => o.TotalPrice)
                        : 0;

                ViewBag.MostPopularPackage = _context.OrderItems
                    .Where(i => catererFoodIds.Contains(i.FoodId))
                    .GroupBy(i => i.Name)
                    .Select(g => new
                    {
                        Name = g.Key,
                        Count = g.Count()
                    })
                    .OrderByDescending(x => x.Count)
                    .FirstOrDefault();

                ViewBag.RecentOrders = catererOrders
                    .OrderByDescending(o => o.CreatedAt)
                    .Take(5)
                    .ToList();

                ViewBag.RecentMenuLogs = _context.AppLogs
                    .Where(x => x.UserEmail == catererEmail && x.EventType == "Menu")
                    .OrderByDescending(x => x.CreatedAt)
                    .Take(5)
                    .ToList();

                return View("CatererDashboard");
            }

            if (User.IsInRole("User"))
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                ViewBag.TotalOrders =
                    _context.Orders.Count(x => x.UserId == userId);

                ViewBag.CompletedOrders =
                    _context.Orders.Count(x => x.UserId == userId && x.Status == "Completed");

                ViewBag.TotalSpent =
                    _context.Orders.Any(x => x.UserId == userId)
                        ? _context.Orders.Where(x => x.UserId == userId).Sum(x => x.TotalPrice)
                        : 0;

                ViewBag.LatestOrder =
                    _context.Orders
                        .Where(x => x.UserId == userId)
                        .OrderByDescending(x => x.CreatedAt)
                        .FirstOrDefault();

                ViewBag.RecentLogs =
                    _context.AppLogs
                        .Where(x => x.UserId == userId)
                        .OrderByDescending(x => x.CreatedAt)
                        .Take(5)
                        .ToList();

                return View("UserDashboard");
            }

            return RedirectToAction("Index", "Home");
        }
    }
}