using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using YemekAlemi.Data;
using YemekAlemi.Models;
using YemekAlemi.Helpers;
using Microsoft.EntityFrameworkCore;

namespace YemekAlemi.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _context;

    public HomeController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult Index(double? userLat, double? userLng, double? maxDistanceKm, string? search)
    {
        var foods = _context.Foods
    .Include(f => f.CustomizationOptions)
    .ToList();
        if (!string.IsNullOrEmpty(search))
        {
            foods = foods
                .Where(f =>
                    f.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    (!string.IsNullOrEmpty(f.RestaurantName) &&
                     f.RestaurantName.Contains(search, StringComparison.OrdinalIgnoreCase)))
                .ToList();
        }

        ViewBag.Search = search;

        if (userLat.HasValue && userLng.HasValue && maxDistanceKm.HasValue)
        {
            foods = foods
                .Where(f => f.Latitude != 0 && f.Longitude != 0)
                .Where(f => CalculateDistanceKm(userLat.Value, userLng.Value, f.Latitude, f.Longitude) <= maxDistanceKm.Value)
                .ToList();
        }

        ViewBag.UserLat = userLat;
        ViewBag.UserLng = userLng;
        ViewBag.MaxDistanceKm = maxDistanceKm;
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

        var restaurants = foods
    .Where(f => !string.IsNullOrWhiteSpace(f.RestaurantName))
    .GroupBy(f => f.RestaurantName.Trim().ToLower())
    .Select(g => g.First())
    .ToList();

        return View(restaurants);
    }

    private double CalculateDistanceKm(double lat1, double lon1, double lat2, double lon2)
    {
        double R = 6371;

        double dLat = ToRadians(lat2 - lat1);
        double dLon = ToRadians(lon2 - lon1);

        double a =
            Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
            Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
            Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return R * c;
    }

    private double ToRadians(double degree)
    {
        return degree * Math.PI / 180;
    }
    [HttpPost]
    public IActionResult AddToCart(int id, int customizationOptionId)
    {
        var food = _context.Foods.FirstOrDefault(f => f.Id == id);

        if (food == null)
        {
            return NotFound();
        }

        string customizationName = "No customization";
        double customizationPrice = 0;

        if (customizationOptionId != 0)
        {
            var option = _context.CustomizationOptions
                .FirstOrDefault(o => o.Id == customizationOptionId && o.FoodId == id);

            if (option != null)
            {
                customizationName = option.Name;
                customizationPrice = (double)option.ExtraPrice;
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
            Message = $"{food.Name} added to cart. Customization: {customizationName}",
            UserEmail = User.Identity?.Name,
            CreatedAt = DateTime.Now
        });

        _context.SaveChanges();

        SessionHelper.SetObject(HttpContext.Session, "Cart", cart);

        return RedirectToAction("Index", "Cart");
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
    public IActionResult RestaurantMenus(string restaurantName)
    {
        if (string.IsNullOrEmpty(restaurantName))
        {
            return RedirectToAction("Index");
        }

        var foods = _context.Foods
            .Include(f => f.CustomizationOptions)
            .Where(f => f.RestaurantName == restaurantName)
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
}