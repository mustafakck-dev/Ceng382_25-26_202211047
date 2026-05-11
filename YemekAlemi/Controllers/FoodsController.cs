using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YemekAlemi.Data;
using YemekAlemi.Models;

namespace YemekAlemi.Controllers
{
    [Authorize(Roles = "Caretaker")]
    public class FoodsController : Controller
    {
        private readonly AppDbContext _context;

        public FoodsController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var foods = _context.Foods
    .Include(f => f.CustomizationOptions)
    .ToList();
            return View(foods);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Food food)
        {
            if (ModelState.IsValid)
            {
                _context.Foods.Add(food);

                _context.AppLogs.Add(new AppLog
                {
                    EventType = "Menu",
                    Message = $"{food.Name} menu item created.",
                    UserEmail = User.Identity?.Name,
                    CreatedAt = DateTime.Now
                });

                _context.SaveChanges();

                return RedirectToAction("Index", "Foods");
            }

            return View(food);
        }

        public IActionResult Edit(int id)
        {
            var food = _context.Foods.FirstOrDefault(f => f.Id == id);

            if (food == null)
            {
                return NotFound();
            }

            return View(food);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Food food)
        {
            if (ModelState.IsValid)
            {
                _context.Foods.Update(food);

                _context.AppLogs.Add(new AppLog
                {
                    EventType = "Menu",
                    Message = $"{food.Name} menu item updated.",
                    UserEmail = User.Identity?.Name,
                    CreatedAt = DateTime.Now
                });

                _context.SaveChanges();

                return RedirectToAction("Index", "Foods");
            }

            return View(food);
        }

        public IActionResult Delete(int id)
        {
            var food = _context.Foods.FirstOrDefault(f => f.Id == id);

            if (food == null)
            {
                return NotFound();
            }

            return View(food);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var food = _context.Foods.FirstOrDefault(f => f.Id == id);

            if (food == null)
            {
                return NotFound();
            }

            _context.Foods.Remove(food);

            _context.AppLogs.Add(new AppLog
            {
                EventType = "Menu",
                Message = $"{food.Name} menu item deleted.",
                UserEmail = User.Identity?.Name,
                CreatedAt = DateTime.Now
            });

            _context.SaveChanges();

            return RedirectToAction("Index", "Foods");
        }
    }
}