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

        public PaymentController(
            AppDbContext context,
            LogService logService,
            EmailService emailService)
        {
            _context = context;
            _logService = logService;
            _emailService = emailService;
        }

        public IActionResult Index(
            int guestCount,
            string eventType,
            DateTime? eventDate,
            string eventAddress,
            string? specialRequest)
        {
            var cart =
                SessionHelper.GetObject<List<CartItem>>(HttpContext.Session, "Cart");

            if (cart == null || !cart.Any())
            {
                return RedirectToAction("Index", "Cart");
            }

            HttpContext.Session.SetInt32("GuestCount", guestCount);
            HttpContext.Session.SetString("EventType", eventType ?? "");
            HttpContext.Session.SetString("EventAddress", eventAddress ?? "");
            HttpContext.Session.SetString("SpecialRequest", specialRequest ?? "");

            if (eventDate.HasValue)
            {
                HttpContext.Session.SetString(
                    "EventDate",
                    eventDate.Value.ToString("o"));
            }

            ViewBag.GuestCount = guestCount;
            ViewBag.EventType = eventType;
            ViewBag.EventDate = eventDate;
            ViewBag.EventAddress = eventAddress;
            ViewBag.SpecialRequest = specialRequest;

            return View(cart);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CompletePayment()
        {
            var cart =
                SessionHelper.GetObject<List<CartItem>>(HttpContext.Session, "Cart");

            if (cart == null || !cart.Any())
            {
                return RedirectToAction("Index", "Cart");
            }

            var userId =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            var guestCount =
                HttpContext.Session.GetInt32("GuestCount") ?? 1;

            var order = new Order
            {
                UserId = userId ?? "",

                TotalPrice =
                    cart.Sum(x =>
                        ((x.Price + x.CustomizationPrice) * x.Quantity)
                        * guestCount),

                Status = "Completed",

                CreatedAt = DateTime.Now,

                GuestCount = guestCount,

                EventType =
                    HttpContext.Session.GetString("EventType") ?? "",

                EventAddress =
                    HttpContext.Session.GetString("EventAddress") ?? "",

                SpecialRequest =
                    HttpContext.Session.GetString("SpecialRequest") ?? "",

                Items = new List<OrderItem>()
            };

            var eventDateString =
                HttpContext.Session.GetString("EventDate");

            if (!string.IsNullOrEmpty(eventDateString))
            {
                order.EventDate =
                    DateTime.Parse(eventDateString);
            }

            foreach (var item in cart)
            {
                order.Items.Add(new OrderItem
                {
                    FoodId = item.FoodId,
                    Name = item.Name,
                    Price = item.Price,
                    RestaurantName =
    _context.Foods.FirstOrDefault(f => f.Id == item.FoodId)?.RestaurantName ?? "",
                    Quantity = item.Quantity,
                    Customization = string.IsNullOrEmpty(item.Customization)
                        ? "No customization"
                        : item.Customization,
                    CustomizationPrice = item.CustomizationPrice
                });
            }

            _context.Orders.Add(order);
            _context.SaveChanges();

            _logService.AddLog(
                "Payment",
                $"Catering payment completed. OrderId: {order.Id}, Guests: {order.GuestCount}, Total: {order.TotalPrice}",
                userId,
                User.Identity?.Name
            );

            _logService.AddLog(
                "Order",
                $"Catering order created successfully. OrderId: {order.Id}, EventType: {order.EventType}",
                userId,
                User.Identity?.Name
            );

            var customerEmailBody =
                $"Your catering order has been completed successfully.\n\n" +
                $"Order ID: {order.Id}\n" +
                $"Guest Count: {order.GuestCount}\n" +
                $"Event Type: {order.EventType}\n" +
                $"Event Date: {order.EventDate}\n" +
                $"Event Address: {order.EventAddress}\n" +
                $"Special Request: {order.SpecialRequest}\n" +
                $"Total Price: {order.TotalPrice} ₺\n" +
                $"Status: {order.Status}\n" +
                $"Order Time: {order.CreatedAt}\n\n" +
                $"Selected Catering Packages:\n";

            foreach (var item in order.Items)
            {
                customerEmailBody +=
                    $"- {item.Name} x {item.Quantity}, " +
                    $"Customization: {item.Customization}, " +
                    $"Per Guest Price: {item.Price + item.CustomizationPrice} ₺\n";
            }

            _emailService.SendEmail(
                User.Identity?.Name ?? "unknown@email.com",
                $"Catering Order Confirmation - Order #{order.Id}",
                customerEmailBody,
                userId
            );

            _logService.AddLog(
                "Email",
                $"Customer confirmation email sent to {User.Identity?.Name}. OrderId: {order.Id}",
                userId,
                User.Identity?.Name
            );

            var catererEmails = cart
                .Select(item =>
                    _context.Foods
                        .FirstOrDefault(f => f.Id == item.FoodId)?
                        .CatererEmail)
                .Where(email => !string.IsNullOrWhiteSpace(email))
                .Distinct()
                .ToList();

            foreach (var catererEmail in catererEmails)
            {
                var catererBody =
                    $"A new catering order has been received.\n\n" +
                    $"Order ID: {order.Id}\n" +
                    $"Customer Email: {User.Identity?.Name}\n" +
                    $"Guest Count: {order.GuestCount}\n" +
                    $"Event Type: {order.EventType}\n" +
                    $"Event Date: {order.EventDate}\n" +
                    $"Event Address: {order.EventAddress}\n" +
                    $"Special Request: {order.SpecialRequest}\n" +
                    $"Total Price: {order.TotalPrice} ₺\n\n" +
                    $"Ordered Catering Packages:\n";

                foreach (var item in order.Items)
                {
                    catererBody +=
                        $"- {item.Name} x {item.Quantity}, " +
                        $"Customization: {item.Customization}, " +
                        $"Per Guest Price: {item.Price + item.CustomizationPrice} ₺\n";
                }

                _emailService.SendEmail(
                    catererEmail!,
                    $"New Catering Order Received - Order #{order.Id}",
                    catererBody,
                    userId
                );

                _logService.AddLog(
                    "Email",
                    $"Caterer notification email sent to {catererEmail}. OrderId: {order.Id}",
                    userId,
                    User.Identity?.Name
                );
            }

            _emailService.SendEmail(
                "mkucuk202@gmail.com",
                $"Admin Notification - New Catering Order #{order.Id}",
                $"A new catering order was completed.\n\n" +
                $"Order ID: {order.Id}\n" +
                $"Customer: {User.Identity?.Name}\n" +
                $"Guest Count: {order.GuestCount}\n" +
                $"Event Type: {order.EventType}\n" +
                $"Event Date: {order.EventDate}\n" +
                $"Event Address: {order.EventAddress}\n" +
                $"Total Price: {order.TotalPrice} ₺",
                userId
            );

            _logService.AddLog(
                "Email",
                $"Admin notification email sent for OrderId: {order.Id}",
                userId,
                User.Identity?.Name
            );

            HttpContext.Session.Remove("Cart");
            HttpContext.Session.Remove("GuestCount");
            HttpContext.Session.Remove("EventType");
            HttpContext.Session.Remove("EventDate");
            HttpContext.Session.Remove("EventAddress");
            HttpContext.Session.Remove("SpecialRequest");

            return RedirectToAction("Success", new { orderId = order.Id });
        }

        public IActionResult Success(int orderId)
        {
            ViewBag.OrderId = orderId;
            return View();
        }
    }
}