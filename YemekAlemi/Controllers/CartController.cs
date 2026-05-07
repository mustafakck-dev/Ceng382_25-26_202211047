using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YemekAlemi.Helpers;
using YemekAlemi.Models;

namespace YemekAlemi.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
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
                cart.Remove(item);
            }

            SessionHelper.SetObject(HttpContext.Session, "Cart", cart);

            return RedirectToAction("Index");
        }

        public IActionResult Clear()
        {
            HttpContext.Session.Remove("Cart");

            return RedirectToAction("Index");
        }
    }
}