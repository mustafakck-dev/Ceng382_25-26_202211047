using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YemekAlemi.Data;
using YemekAlemi.Models;

namespace YemekAlemi.Controllers
{
    [Authorize(Roles = "Caterer")]
    public class CustomizationOptionsController : Controller
    {
        private readonly AppDbContext _context;

        public CustomizationOptionsController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Create(int foodId)
        {
            var food = _context.Foods.FirstOrDefault(f => f.Id == foodId);

            if (food == null)
            {
                return NotFound();
            }

            ViewBag.FoodId = food.Id;
            ViewBag.FoodName = food.Name;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CustomizationOption option)
        {
            if (ModelState.IsValid)
            {
                _context.CustomizationOptions.Add(option);

                _context.AppLogs.Add(new AppLog
                {
                    EventType = "Menu",
                    Message = $"{option.Name} customization option added.",
                    UserEmail = User.Identity?.Name,
                    CreatedAt = DateTime.Now
                });

                _context.SaveChanges();

                return RedirectToAction("Index", "Foods");
            }

            return View(option);
        }

        public IActionResult Delete(int id)
        {
            var option = _context.CustomizationOptions
                .Include(o => o.Food)
                .FirstOrDefault(o => o.Id == id);

            if (option == null)
            {
                return NotFound();
            }

            return View(option);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var option = _context.CustomizationOptions.FirstOrDefault(o => o.Id == id);

            if (option == null)
            {
                return NotFound();
            }

            _context.CustomizationOptions.Remove(option);

            _context.AppLogs.Add(new AppLog
            {
                EventType = "Menu",
                Message = $"{option.Name} customization option deleted.",
                UserEmail = User.Identity?.Name,
                CreatedAt = DateTime.Now
            });

            _context.SaveChanges();

            return RedirectToAction("Index", "Foods");
        }
    }
}