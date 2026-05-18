using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YemekAlemi.Data;
using YemekAlemi.Models;

namespace YemekAlemi.Controllers
{
    [Authorize(Roles = "Caterer")]
    public class FoodsController : Controller
    {
        private readonly AppDbContext _context;

        public FoodsController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(int page = 1)
        {
            var catererEmail = User.Identity?.Name;

            int pageSize = 5;

            var query = _context.Foods
                .Include(f => f.CustomizationOptions)
                .Where(f => f.CatererEmail == catererEmail)
                .OrderBy(f => f.RestaurantName)
                .ThenBy(f => f.Name);

            int totalItems = query.Count();

            var foods = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            return View(foods);
        }

        public IActionResult Create()
        {
            var catererEmail = User.Identity?.Name;

            var company = _context.CateringCompanies
    .FirstOrDefault(x => x.OwnerEmail == catererEmail);

            if (company != null)
            {
                ViewBag.CompanyName = company.CompanyName;
                ViewBag.CompanyAddress = company.Address;
                ViewBag.Latitude = company.Latitude;
                ViewBag.Longitude = company.Longitude;
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Food food)
        {
            var catererEmail = User.Identity?.Name;

            var company = _context.CateringCompanies
    .FirstOrDefault(x => x.OwnerEmail == catererEmail);

            if (company == null)
            {
                return RedirectToAction("Index", "Foods");
            }

            if (ModelState.IsValid)
            {
                food.CatererEmail = catererEmail;
                food.RestaurantName = company.CompanyName;
                food.Address = company.Address;
                food.Latitude = company.Latitude;
                food.Longitude = company.Longitude;

                _context.Foods.Add(food);

                _context.AppLogs.Add(new AppLog
                {
                    EventType = "Menu",
                    Message = $"{food.Name} catering package created by {catererEmail}.",
                    UserEmail = catererEmail,
                    CreatedAt = DateTime.Now
                });

                _context.SaveChanges();

                return RedirectToAction("Index", "Foods");
            }

            return View(food);
        }

        public IActionResult Edit(int id)
        {
            var currentCatererEmail = User.Identity?.Name;

            var food = _context.Foods
                .FirstOrDefault(f =>
                    f.Id == id &&
                    f.CatererEmail == currentCatererEmail);

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
            var currentCatererEmail = User.Identity?.Name;

            var existingFood = _context.Foods
                .FirstOrDefault(f =>
                    f.Id == food.Id &&
                    f.CatererEmail == currentCatererEmail);

            if (existingFood == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                existingFood.Name = food.Name;
                existingFood.Price = food.Price;
                existingFood.Description = food.Description;
                existingFood.PackageContents = food.PackageContents;
                existingFood.RestaurantName = food.RestaurantName;
                existingFood.Address = food.Address;
                existingFood.Latitude = food.Latitude;
                existingFood.Longitude = food.Longitude;
                existingFood.ImageUrl = food.ImageUrl;

                _context.AppLogs.Add(new AppLog
                {
                    EventType = "Menu",
                    Message = $"{existingFood.Name} catering package updated by {currentCatererEmail}.",
                    UserEmail = currentCatererEmail,
                    CreatedAt = DateTime.Now
                });

                _context.SaveChanges();

                return RedirectToAction("Index", "Foods");
            }

            return View(food);
        }

        public IActionResult Delete(int id)
        {
            var currentCatererEmail = User.Identity?.Name;

            var food = _context.Foods
                .FirstOrDefault(f =>
                    f.Id == id &&
                    f.CatererEmail == currentCatererEmail);

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
            var currentCatererEmail = User.Identity?.Name;

            var food = _context.Foods
                .FirstOrDefault(f =>
                    f.Id == id &&
                    f.CatererEmail == currentCatererEmail);

            if (food == null)
            {
                return NotFound();
            }

            _context.Foods.Remove(food);

            _context.AppLogs.Add(new AppLog
            {
                EventType = "Menu",
                Message = $"{food.Name} catering package deleted by {currentCatererEmail}.",
                UserEmail = currentCatererEmail,
                CreatedAt = DateTime.Now
            });

            _context.SaveChanges();

            return RedirectToAction("Index", "Foods");
        }
    }
}