using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using YemekAlemi.Data;
using YemekAlemi.Helpers;
using YemekAlemi.Models;
using YemekAlemi.Services;

namespace YemekAlemi.Controllers
{
    [Authorize]
    public class PaymentController : Controller
    {
        private readonly AppDbContext _context;
        private readonly LogService _logService;
        private readonly EmailService _emailService;

        public PaymentController(AppDbContext context, LogService logService, EmailService emailService)
        {
            _context = context;
            _logService = logService;
            _emailService = emailService;
        }

        public IActionResult Index()
        {
            var cart = SessionHelper.GetObject<List<CartItem>>(HttpContext.Session, "Cart");

            if (cart == null || !cart.Any())
            {
                return RedirectToAction("Index", "Cart");
            }

            return View(cart);
        }

        [HttpPost]
        public IActionResult CompletePayment()
        {
            var cart = SessionHelper.GetObject<List<CartItem>>(HttpContext.Session, "Cart");

            if (cart == null || !cart.Any())
            {
                return RedirectToAction("Index", "Cart");
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var order = new Order
            {
                UserId = userId,
                TotalPrice = cart.Sum(x => x.TotalPrice),
                Status = "Completed",
                CreatedAt = DateTime.Now,
                Items = new List<OrderItem>()
            };

            foreach (var item in cart)
            {
                order.Items.Add(new OrderItem
                {
                    FoodId = item.FoodId,
                    Name = item.Name,
                    Price = item.Price,
                    Quantity = item.Quantity,
                    Customization = string.IsNullOrEmpty(item.Customization) ? "No customization" : item.Customization,
                    CustomizationPrice = item.CustomizationPrice
                });
            }

            _context.Orders.Add(order);
            _context.SaveChanges();
            _logService.AddLog(
          "Payment",
          $"Payment completed. OrderId: {order.Id}, Total: {order.TotalPrice}",
          userId,
          User.Identity?.Name
        );
            _logService.AddLog(
        "Order",
        $"Order created successfully. OrderId: {order.Id}",
        userId,
        User.Identity?.Name
    );
            var emailBody = $"Your order has been completed successfully.\n\n" +
                        $"Order ID: {order.Id}\n" +
                        $"Total Price: {order.TotalPrice} ₺\n" +
                        $"Status: {order.Status}\n" +
                        $"Order Time: {order.CreatedAt}\n\n" +
                        $"Items:\n";

            foreach (var item in order.Items)
            {
                emailBody += $"- {item.Name} x {item.Quantity}, Customization: {item.Customization}, Price: {item.Price} ₺\n";
            }

            _emailService.SendEmail(
                User.Identity?.Name ?? "unknown@email.com",
                $"Order Confirmation - Order #{order.Id}",
                emailBody,
                userId
            );


            HttpContext.Session.Remove("Cart");

            return RedirectToAction("Success");
        }

        public IActionResult Success()
        {
            return View();
        }
    }
}