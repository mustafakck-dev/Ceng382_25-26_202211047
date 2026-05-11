using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using YemekAlemi.Data;
using YemekAlemi.Services;

namespace YemekAlemi.Controllers
{
    [Authorize]
    public class DocumentsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly LogService _logService;

        public DocumentsController(AppDbContext context, LogService logService)
        {
            _context = context;
            _logService = logService;
        }

        public IActionResult Receipt(int orderId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var order = _context.Orders
                .Include(o => o.Items)
                .FirstOrDefault(o => o.Id == orderId && o.UserId == userId);

            if (order == null)
            {
                return NotFound();
            }

            _logService.AddLog(
                "Document",
                $"Receipt viewed for OrderId: {order.Id}",
                userId,
                User.Identity?.Name
            );

            return View(order);
        }

        public IActionResult Agreement(int orderId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var order = _context.Orders
                .Include(o => o.Items)
                .FirstOrDefault(o => o.Id == orderId && o.UserId == userId);

            if (order == null)
            {
                return NotFound();
            }

            _logService.AddLog(
                "Document",
                $"Agreement viewed for OrderId: {order.Id}",
                userId,
                User.Identity?.Name
            );

            return View(order);
        }
    }
}