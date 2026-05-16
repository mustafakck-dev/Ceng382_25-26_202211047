using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YemekAlemi.Data;
using YemekAlemi.Models;

namespace YemekAlemi.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminCompaniesController : Controller
    {
        private readonly AppDbContext _context;

        public AdminCompaniesController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(
            string companyName,
            string ownerEmail,
            string address,
            double latitude,
            double longitude,
            string description,
            string imageUrl)
        {
            if (string.IsNullOrWhiteSpace(companyName) ||
                string.IsNullOrWhiteSpace(ownerEmail))
            {
                ViewBag.Error = "Company name and owner email are required.";
                return View();
            }

            bool companyExists = _context.Foods
                .Any(f => f.RestaurantName == companyName);

            if (companyExists)
            {
                ViewBag.Error = "This catering company already exists.";
                return View();
            }

            var starterPackage = new Food
            {
                Name = "Starter Catering Package",
                Description = description,
                PackageContents = "Main dish, side dish, salad, dessert and beverage service",
                Price = 150,
                RestaurantName = companyName,
                Address = address,
                Latitude = latitude,
                Longitude = longitude,
                ImageUrl = imageUrl,
                CatererEmail = ownerEmail
            };

            _context.Foods.Add(starterPackage);

            _context.AppLogs.Add(new AppLog
            {
                EventType = "Menu",
                Message = $"New catering company created: {companyName}. Owner: {ownerEmail}",
                UserEmail = User.Identity?.Name,
                CreatedAt = DateTime.Now
            });

            _context.SaveChanges();

            return RedirectToAction("Index", "Dashboard");
        }
    }
}