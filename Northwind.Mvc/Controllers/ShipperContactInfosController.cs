using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Northwind.Mvc.Models;

namespace Northwind.Mvc.Controllers
{
    public class ShipperContactInfosController : Controller
    {
        private readonly NorthwindContext _context;

        public ShipperContactInfosController(NorthwindContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var data = _context.ShipperContactInfos
                               .Include(s => s.Shipper);

            return View(await data.ToListAsync());
        }
    }
}