using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
                _context.SaveChanges();
                return RedirectToAction("Index", "Home");
            }

            return View(food);
        }
    }
}