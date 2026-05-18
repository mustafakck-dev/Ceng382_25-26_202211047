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

        public IActionResult Index()
        {
            var companies = _context.CateringCompanies
                .OrderBy(x => x.CompanyName)
                .ToList();

            return View(companies);
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

            bool companyExists = _context.CateringCompanies
                .Any(x => x.CompanyName == companyName);

            if (companyExists)
            {
                ViewBag.Error = "This catering company already exists.";
                return View();
            }

            var company = new CateringCompany
            {
                CompanyName = companyName,
                OwnerEmail = ownerEmail,
                Address = address,
                Latitude = latitude,
                Longitude = longitude,
                Description = description,
                ImageUrl = imageUrl,
                CreatedAt = DateTime.Now
            };

            _context.CateringCompanies.Add(company);

            _context.AppLogs.Add(new AppLog
            {
                EventType = "Company",
                Message = $"New catering company created: {companyName}. Owner: {ownerEmail}",
                UserEmail = User.Identity?.Name,
                CreatedAt = DateTime.Now
            });

            _context.SaveChanges();

            return RedirectToAction("Index", "AdminCompanies");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var company = _context.CateringCompanies
                .FirstOrDefault(x => x.Id == id);

            if (company == null)
            {
                return NotFound();
            }

            var foods = _context.Foods
                .Where(f => f.RestaurantName == company.CompanyName)
                .ToList();

            _context.Foods.RemoveRange(foods);
            _context.CateringCompanies.Remove(company);

            _context.AppLogs.Add(new AppLog
            {
                EventType = "Company",
                Message = $"Catering company deleted: {company.CompanyName}",
                UserEmail = User.Identity?.Name,
                CreatedAt = DateTime.Now
            });

            _context.SaveChanges();

            return RedirectToAction("Index", "AdminCompanies");
        }
    }
}