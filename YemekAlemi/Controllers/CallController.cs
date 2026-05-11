using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using YemekAlemi.Data;
using YemekAlemi.Models;

namespace YemekAlemi.Controllers
{
    [Authorize]
    public class CallController : Controller
    {
        private readonly AppDbContext _context;

        public CallController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Room(int orderId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var order = _context.Orders
                .Include(o => o.Items)
                .FirstOrDefault(o => o.Id == orderId);

            if (order == null)
            {
                return NotFound();
            }

            if (User.IsInRole("User") && order.UserId != userId)
            {
                return Forbid();
            }

            if (!User.IsInRole("User") && !User.IsInRole("Caretaker") && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            ViewBag.OrderId = order.Id;
            ViewBag.CustomerId = order.UserId;
            ViewBag.TotalPrice = order.TotalPrice;
            ViewBag.Status = order.Status;
            ViewBag.CreatedAt = order.CreatedAt;

            var firstItem = order.Items.FirstOrDefault();

            if (firstItem != null)
            {
                var food = _context.Foods.FirstOrDefault(f => f.Id == firstItem.FoodId);
                ViewBag.RestaurantName = food?.RestaurantName ?? "Restaurant";
            }
            else
            {
                ViewBag.RestaurantName = "Restaurant";
            }

            _context.AppLogs.Add(new AppLog
            {
                EventType = "Call",
                Message = $"Live call room opened for OrderId: {order.Id}",
                UserEmail = User.Identity?.Name,
                CreatedAt = DateTime.Now
            });

            _context.SaveChanges();

            return View();
        }
    }
}