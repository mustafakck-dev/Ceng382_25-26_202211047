using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using YemekAlemi.Data;
using YemekAlemi.Models;
using YemekAlemi.Services;

namespace YemekAlemi.Controllers
{
    [Authorize(Roles = "User")]
    public class RatingsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly LogService _logService;

        public RatingsController(AppDbContext context, LogService logService)
        {
            _context = context;
            _logService = logService;
        }

        public IActionResult MyCompletedOrders(string? search, int page = 1)
        {
            int pageSize = 5;

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var orders = _context.Orders
                .Include(o => o.Items)
                .Where(o => o.UserId == userId && o.Status == "Completed")
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                orders = orders.Where(o =>
                    o.Id.ToString().Contains(search) ||
                    o.TotalPrice.ToString().Contains(search) ||
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
            ViewBag.TotalPages = (int)Math.Ceiling(totalOrders / (double)pageSize);

            return View(pagedOrders);
        }

        public IActionResult Create(int orderId, int foodId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var order = _context.Orders
                .Include(o => o.Items)
                .FirstOrDefault(o => o.Id == orderId && o.UserId == userId && o.Status == "Completed");

            if (order == null)
            {
                return NotFound();
            }

            var alreadyRated = _context.Ratings
                .Any(r => r.OrderId == orderId && r.FoodId == foodId && r.UserId == userId);

            if (alreadyRated)
            {
                TempData["Message"] = "You have already rated this item.";
                return RedirectToAction("MyCompletedOrders");
            }

            var item = order.Items.FirstOrDefault(i => i.FoodId == foodId);

            if (item == null)
            {
                return NotFound();
            }

            ViewBag.OrderId = orderId;
            ViewBag.FoodId = foodId;
            ViewBag.FoodName = item.Name;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Rating rating)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var order = _context.Orders
                .Include(o => o.Items)
                .FirstOrDefault(o => o.Id == rating.OrderId && o.UserId == userId && o.Status == "Completed");

            if (order == null)
            {
                return NotFound();
            }

            var itemExists = order.Items.Any(i => i.FoodId == rating.FoodId);

            if (!itemExists)
            {
                return NotFound();
            }

            var alreadyRated = _context.Ratings
                .Any(r => r.OrderId == rating.OrderId && r.FoodId == rating.FoodId && r.UserId == userId);

            if (alreadyRated)
            {
                TempData["Message"] = "You have already rated this item.";
                return RedirectToAction("MyCompletedOrders");
            }

            rating.UserId = userId;
            rating.CreatedAt = DateTime.Now;

            ModelState.Remove("UserId");

            if (ModelState.IsValid)
            {
                _context.Ratings.Add(rating);
                _context.SaveChanges();

                _logService.AddLog(
                    "Rating",
                    $"Rating submitted. OrderId: {rating.OrderId}, FoodId: {rating.FoodId}, MenuRating: {rating.MenuRating}, CatererRating: {rating.CatererRating}",
                    userId,
                    User.Identity?.Name
                );

                return RedirectToAction("MyCompletedOrders");
            }

            return View(rating);
        }
    }
}