using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YemekAlemi.Helpers;
using YemekAlemi.Models;
using YemekAlemi.Data;

namespace YemekAlemi.Controllers
{
    [Authorize(Roles = "User")]
    public class CartController : Controller
    {
        private readonly AppDbContext _context;

        public CartController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var cart = SessionHelper.GetObject<List<CartItem>>(HttpContext.Session, "Cart") ?? new List<CartItem>();

            return View(cart);
        }

        public IActionResult Increase(int foodId, string customization)
        {
            var cart = SessionHelper.GetObject<List<CartItem>>(HttpContext.Session, "Cart") ?? new List<CartItem>();

            var item = cart.FirstOrDefault(x => x.FoodId == foodId && x.Customization == customization);

            if (item != null)
            {
                item.Quantity++;
                _context.AppLogs.Add(new AppLog
                {
                    EventType = "Cart",
                    Message = $"Increased quantity for FoodId: {foodId}",
                    UserEmail = User.Identity?.Name,
                    CreatedAt = DateTime.Now
                });
            }

            SessionHelper.SetObject(HttpContext.Session, "Cart", cart);

            return RedirectToAction("Index");
        }

        public IActionResult Decrease(int foodId, string customization)
        {
            var cart = SessionHelper.GetObject<List<CartItem>>(HttpContext.Session, "Cart") ?? new List<CartItem>();

            var item = cart.FirstOrDefault(x => x.FoodId == foodId && x.Customization == customization);

            if (item != null)
            {
                item.Quantity--;
                _context.AppLogs.Add(new AppLog
                {
                    EventType = "Cart",
                    Message = $"Decreased quantity for FoodId: {foodId}",
                    UserEmail = User.Identity?.Name,
                    CreatedAt = DateTime.Now
                });

                if (item.Quantity <= 0)
                {
                    cart.Remove(item);
                }
            }

            SessionHelper.SetObject(HttpContext.Session, "Cart", cart);

            return RedirectToAction("Index");
        }

        public IActionResult Remove(int foodId, string customization)
        {
            var cart = SessionHelper.GetObject<List<CartItem>>(HttpContext.Session, "Cart") ?? new List<CartItem>();

            var item = cart.FirstOrDefault(x => x.FoodId == foodId && x.Customization == customization);

            if (item != null)
            {
                _context.AppLogs.Add(new AppLog
                {
                    EventType = "Cart",
                    Message = $"Removed item from cart. FoodId: {foodId}",
                    UserEmail = User.Identity?.Name,
                    CreatedAt = DateTime.Now
                });
                cart.Remove(item);
            }

            SessionHelper.SetObject(HttpContext.Session, "Cart", cart);

            return RedirectToAction("Index");
        }

        public IActionResult Clear()
        {
            _context.AppLogs.Add(new AppLog
            {
                EventType = "Cart",
                Message = "Cart cleared",
                UserEmail = User.Identity?.Name,
                CreatedAt = DateTime.Now
            });

            _context.SaveChanges();
            HttpContext.Session.Remove("Cart");

            return RedirectToAction("Index");
        }
    }
}