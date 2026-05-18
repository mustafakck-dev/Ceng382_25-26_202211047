using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YemekAlemi.Data;
using YemekAlemi.Helpers;
using YemekAlemi.Models;

namespace YemekAlemi.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _context;

    public HomeController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult Index(
        double? userLat,
        double? userLng,
        double? maxDistanceKm,
        string? search,
        string? company,
       string? packageType,
         string? district)
    {
        var foods = _context.Foods
            .Include(f => f.CustomizationOptions)
            .ToList();

        if (!string.IsNullOrWhiteSpace(district))
        {
            var normalizedDistrict = NormalizeText(district);

            foods = foods
                .Where(f =>
                    !string.IsNullOrEmpty(f.Address) &&
                    NormalizeText(f.Address).Contains(normalizedDistrict))
                .ToList();
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            foods = foods
                .Where(f =>
                    f.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    (!string.IsNullOrEmpty(f.RestaurantName) &&
                     f.RestaurantName.Contains(search, StringComparison.OrdinalIgnoreCase)))
                .ToList();
        }

        if (!string.IsNullOrWhiteSpace(company))
        {
            foods = foods
                .Where(f => f.RestaurantName == company)
                .ToList();
        }

        if (!string.IsNullOrWhiteSpace(packageType))
        {
            foods = foods
                .Where(f =>
                    f.Name.Contains(packageType, StringComparison.OrdinalIgnoreCase) ||
                    (!string.IsNullOrEmpty(f.Description) &&
                     f.Description.Contains(packageType, StringComparison.OrdinalIgnoreCase)))
                .ToList();
        }

        if (userLat.HasValue && userLng.HasValue && maxDistanceKm.HasValue)
        {
            foods = foods
                .Where(f => f.Latitude != 0 && f.Longitude != 0)
                .Where(f =>
                    CalculateDistanceKm(
                        userLat.Value,
                        userLng.Value,
                        f.Latitude,
                        f.Longitude) <= maxDistanceKm.Value)
                .ToList();
        }

        ViewBag.Search = search;
        ViewBag.SelectedCompany = company;
        ViewBag.SelectedPackageType = packageType;

        ViewBag.UserLat = userLat;
        ViewBag.UserLng = userLng;
        ViewBag.MaxDistanceKm = maxDistanceKm;

        ViewBag.Companies = _context.Foods
            .Where(f => !string.IsNullOrWhiteSpace(f.RestaurantName))
            .Select(f => f.RestaurantName)
            .Distinct()
            .OrderBy(x => x)
            .ToList();

        ViewBag.PackageTypes = new List<string>
        {
            "Wedding",
            "Corporate",
            "Luxury",
            "Traditional",
            "Dessert",
            "Healthy",
            "Street Food",
            "Daily Meal",
            "Breakfast",
            "Fast Food"
        };
        ViewBag.SelectedDistrict = district;

        ViewBag.Districts = new List<string>
{
    "Çankaya",
    "Keçiören",
    "Yenimahalle",
    "Etimesgut",
    "Mamak",
    "Sincan",
    "Gölbaşı",
    "Pursaklar"
};

        ViewBag.MenuRatings = _context.Ratings
            .GroupBy(r => r.FoodId)
            .Select(g => new
            {
                FoodId = g.Key,
                Average = Math.Round(g.Average(x => x.MenuRating), 1)
            })
            .ToDictionary(x => x.FoodId, x => x.Average);

        ViewBag.TopRatedFoods = _context.Foods
            .Select(f => new
            {
                Food = f,
                AverageRating = _context.Ratings
                    .Where(r => r.FoodId == f.Id)
                    .Average(r => (double?)r.MenuRating) ?? 0
            })
            .OrderByDescending(x => x.AverageRating)
            .Take(3)
            .ToList();

        var cateringCompanies = foods
            .Where(f => !string.IsNullOrWhiteSpace(f.RestaurantName))
            .GroupBy(f => f.RestaurantName.Trim().ToLower())
            .Select(g => g.First())
            .ToList();

        return View(cateringCompanies);
    }

    public IActionResult RestaurantMenus(string restaurantName)
    {
        if (string.IsNullOrWhiteSpace(restaurantName))
        {
            return RedirectToAction("Index");
        }

        var normalizedName = restaurantName.Trim().ToLower();

        var foods = _context.Foods
            .Include(f => f.CustomizationOptions)
            .Where(f => f.RestaurantName.Trim().ToLower() == normalizedName)
            .ToList();

        ViewBag.RestaurantName = restaurantName;

        ViewBag.MenuRatings = _context.Ratings
            .GroupBy(r => r.FoodId)
            .Select(g => new
            {
                FoodId = g.Key,
                Average = Math.Round(g.Average(x => x.MenuRating), 1)
            })
            .ToDictionary(x => x.FoodId, x => x.Average);

        return View(foods);
    }

    [HttpPost]
    public IActionResult AddToCart(int id, int[] customizationOptionIds)
    {
        var food = _context.Foods.FirstOrDefault(f => f.Id == id);

        if (food == null)
        {
            return NotFound();
        }

        string customizationName = "No additional service";
        double customizationPrice = 0;

        if (customizationOptionIds != null && customizationOptionIds.Any())
        {
            var selectedOptions = _context.CustomizationOptions
                .Where(o => customizationOptionIds.Contains(o.Id) && o.FoodId == id)
                .ToList();

            if (selectedOptions.Any())
            {
                customizationName = string.Join(", ", selectedOptions.Select(o => o.Name));
                customizationPrice = selectedOptions.Sum(o => (double)o.ExtraPrice);
            }
        }

        var cart = SessionHelper.GetObject<List<CartItem>>(HttpContext.Session, "Cart")
            ?? new List<CartItem>();

        var existingItem = cart.FirstOrDefault(x =>
            x.FoodId == food.Id &&
            x.Customization == customizationName);

        if (existingItem != null)
        {
            existingItem.Quantity++;
        }
        else
        {
            cart.Add(new CartItem
            {
                FoodId = food.Id,
                Name = food.Name,
                Price = food.Price,
                Quantity = 1,
                Customization = customizationName,
                CustomizationPrice = customizationPrice
            });
        }

        _context.AppLogs.Add(new AppLog
        {
            EventType = "Cart",
            Message = $"{food.Name} package added to catering cart. Customization: {customizationName}",
            UserEmail = User.Identity?.Name,
            CreatedAt = DateTime.Now
        });

        _context.SaveChanges();

        SessionHelper.SetObject(HttpContext.Session, "Cart", cart);

        return RedirectToAction("Index", "Cart");
    }

    private double CalculateDistanceKm(
        double lat1,
        double lon1,
        double lat2,
        double lon2)
    {
        double radius = 6371;

        double dLat = ToRadians(lat2 - lat1);
        double dLon = ToRadians(lon2 - lon1);

        double a =
            Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
            Math.Cos(ToRadians(lat1)) *
            Math.Cos(ToRadians(lat2)) *
            Math.Sin(dLon / 2) *
            Math.Sin(dLon / 2);

        double c =
            2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return radius * c;
    }

    private double ToRadians(double degree)
    {
        return degree * Math.PI / 180;
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(
        Duration = 0,
        Location = ResponseCacheLocation.None,
        NoStore = true)]
    public IActionResult Error()
    {
        return View(
            new ErrorViewModel
            {
                RequestId =
                    Activity.Current?.Id ??
                    HttpContext.TraceIdentifier
            });
    }
    private string NormalizeText(string text)
    {
        return text
            .ToLower()
            .Replace("ı", "i")
            .Replace("ğ", "g")
            .Replace("ü", "u")
            .Replace("ş", "s")
            .Replace("ö", "o")
            .Replace("ç", "c");
    }
}