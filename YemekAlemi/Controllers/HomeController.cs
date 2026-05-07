using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using YemekAlemi.Data;
using YemekAlemi.Models;
using YemekAlemi.Helpers;

namespace YemekAlemi.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _context;

    public HomeController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult Index(double? userLat, double? userLng, double? maxDistanceKm)
    {
        var foods = _context.Foods.ToList();

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

        return View(foods);
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
    public IActionResult AddToCart(int id, string customization)
    {
        var food = _context.Foods.FirstOrDefault(f => f.Id == id);

        if (food == null)
        {
            return NotFound();
        }

        double customizationPrice = 0;

        if (customization == "Extra Cheese")
        {
            customizationPrice = 20;
        }
        else if (customization == "Extra Sauce")
        {
            customizationPrice = 10;
        }
        else if (customization == "Large Size")
        {
            customizationPrice = 30;
        }

        var cart = SessionHelper.GetObject<List<CartItem>>(HttpContext.Session, "Cart") ?? new List<CartItem>();

        var existingItem = cart.FirstOrDefault(x =>
            x.FoodId == food.Id &&
            x.Customization == customization);

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
                Customization = customization,
                CustomizationPrice = customizationPrice
            });
        }

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
}