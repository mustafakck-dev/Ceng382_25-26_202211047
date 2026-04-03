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

        public async Task<IActionResult> Index(int? editId)
        {
            var vm = new ShipperContactInfoPageViewModel
            {
                ActiveItems = await _context.ShipperContactInfos
                    .Include(x => x.Shipper)
                    .Where(x => x.IsDeleted == false)
                    .ToListAsync(),

                DeletedItems = await _context.ShipperContactInfos
                    .Include(x => x.Shipper)
                    .Where(x => x.IsDeleted == true)
                    .ToListAsync()
            };

            if (editId != null)
            {
                vm.EditItem = await _context.ShipperContactInfos
                    .FirstOrDefaultAsync(x => x.Id == editId);
            }

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateInline(ShipperContactInfo item)
        {
            var existingItem = await _context.ShipperContactInfos
                .FirstOrDefaultAsync(x => x.Id == item.Id);

            if (existingItem == null)
                return NotFound();

            existingItem.ShipperId = item.ShipperId;
            existingItem.Phone = item.Phone;
            existingItem.Email = item.Email;
            existingItem.Address = item.Address;
            existingItem.City = item.City;
            existingItem.Country = item.Country;
            existingItem.PostalCode = item.PostalCode;
            existingItem.Website = item.Website;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> HardDelete(int id)
        {
            var item = await _context.ShipperContactInfos.FindAsync(id);

            if (item != null)
            {
                _context.ShipperContactInfos.Remove(item);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SoftDelete(int id)
        {
            var item = await _context.ShipperContactInfos.FindAsync(id);

            if (item != null)
            {
                item.IsDeleted = true;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}