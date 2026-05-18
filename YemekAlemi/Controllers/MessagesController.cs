using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YemekAlemi.Data;
using YemekAlemi.Models;

namespace YemekAlemi.Controllers
{
    [Authorize]
    public class MessagesController : Controller
    {
        private readonly AppDbContext _context;

        public MessagesController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Room(int orderId)
        {
            var currentEmail = User.Identity?.Name;

            var order = _context.Orders
                .Include(o => o.Items)
                .FirstOrDefault(o => o.Id == orderId);

            if (order == null)
            {
                return NotFound();
            }

            var foodIds = order.Items
                .Select(i => i.FoodId)
                .ToList();

            var catererEmails = _context.Foods
                .Where(f => foodIds.Contains(f.Id))
                .Select(f => f.CatererEmail)
                .Where(e => !string.IsNullOrWhiteSpace(e))
                .Distinct()
                .ToList();

            bool isCustomer =
                order.UserId == _context.Users
                    .Where(u => u.Email == currentEmail)
                    .Select(u => u.Id)
                    .FirstOrDefault();

            bool isCaterer =
                catererEmails.Contains(currentEmail);

            bool isAdmin =
                User.IsInRole("Admin");

            if (!isCustomer && !isCaterer && !isAdmin)
            {
                return Forbid();
            }

            ViewBag.Order = order;
            ViewBag.CurrentEmail = currentEmail;
            ViewBag.CatererEmail = catererEmails.FirstOrDefault() ?? "";

            var messages = _context.OrderMessages
                .Where(m => m.OrderId == orderId)
                .OrderBy(m => m.CreatedAt)
                .ToList();

            return View(messages);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SendMessage(int orderId, string messageText)
        {
            if (string.IsNullOrWhiteSpace(messageText))
            {
                return RedirectToAction("Room", new { orderId });
            }

            var currentEmail = User.Identity?.Name ?? "";

            var order = _context.Orders
                .Include(o => o.Items)
                .FirstOrDefault(o => o.Id == orderId);

            if (order == null)
            {
                return NotFound();
            }

            var foodIds = order.Items
                .Select(i => i.FoodId)
                .ToList();

            var catererEmail = _context.Foods
                .Where(f => foodIds.Contains(f.Id))
                .Select(f => f.CatererEmail)
                .FirstOrDefault() ?? "";

            var customerEmail = _context.Users
                .Where(u => u.Id == order.UserId)
                .Select(u => u.Email)
                .FirstOrDefault() ?? "";

            string receiverEmail;

            if (User.IsInRole("User"))
            {
                receiverEmail = catererEmail;
            }
            else if (User.IsInRole("Caterer"))
            {
                receiverEmail = customerEmail;
            }
            else
            {
                receiverEmail = customerEmail;
            }

            _context.OrderMessages.Add(new OrderMessage
            {
                OrderId = orderId,
                SenderEmail = currentEmail,
                ReceiverEmail = receiverEmail,
                MessageText = messageText,
                CreatedAt = DateTime.Now
            });

            _context.AppLogs.Add(new AppLog
            {
                EventType = "Message",
                Message = $"Message sent for OrderId: {orderId} by {currentEmail}.",
                UserEmail = currentEmail,
                CreatedAt = DateTime.Now
            });

            _context.SaveChanges();

            return RedirectToAction("Room", new { orderId });
        }
    }
}