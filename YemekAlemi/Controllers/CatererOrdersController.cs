using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YemekAlemi.Data;

namespace YemekAlemi.Controllers
{
    [Authorize(Roles = "Caterer")]
    public class CatererOrdersController : Controller
    {
        private readonly AppDbContext _context;

        public CatererOrdersController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string? search, int page = 1)
        {
            int pageSize = 5;

            var userEmail = User.Identity?.Name;

            var catererFoodIds = _context.Foods
                .Where(f => f.CatererEmail == userEmail)
                .Select(f => f.Id)
                .ToList();

            var orders = _context.Orders
                .Include(o => o.Items)
                .Where(o =>
                    o.Items.Any(i =>
                        catererFoodIds.Contains(i.FoodId)))
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                orders = orders.Where(o =>
                    o.Id.ToString().Contains(search) ||
                    o.EventType.Contains(search) ||
                    o.EventAddress.Contains(search) ||
                    o.Items.Any(i => i.Name.Contains(search)));
            }

            int totalOrders = orders.Count();

            var pagedOrders = orders
                .OrderByDescending(o => o.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.Search = search;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages =
                (int)Math.Ceiling(totalOrders / (double)pageSize);

            return View(pagedOrders);
        }
    }
}