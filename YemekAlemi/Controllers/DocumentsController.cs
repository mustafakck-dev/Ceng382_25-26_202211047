using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using YemekAlemi.Data;
using YemekAlemi.Services;

namespace YemekAlemi.Controllers
{
    [Authorize]
    public class DocumentsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly LogService _logService;
        private readonly PdfService _pdfService;

        public DocumentsController(
            AppDbContext context,
            LogService logService,
            PdfService pdfService)
        {
            _context = context;
            _logService = logService;
            _pdfService = pdfService;
        }

        public IActionResult Receipt(int orderId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var order = _context.Orders
                .Include(o => o.Items)
                .FirstOrDefault(o => o.Id == orderId && o.UserId == userId);

            if (order == null)
            {
                return NotFound();
            }

            _logService.AddLog(
                "Document",
                $"Receipt viewed for OrderId: {order.Id}",
                userId,
                User.Identity?.Name
            );

            return View(order);
        }

        public IActionResult Agreement(int orderId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var order = _context.Orders
                .Include(o => o.Items)
                .FirstOrDefault(o => o.Id == orderId && o.UserId == userId);

            if (order == null)
            {
                return NotFound();
            }

            _logService.AddLog(
                "Document",
                $"Agreement viewed for OrderId: {order.Id}",
                userId,
                User.Identity?.Name
            );

            return View(order);
        }

        public IActionResult DownloadReceiptPdf(int orderId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var order = _context.Orders
                .Include(o => o.Items)
                .FirstOrDefault(o => o.Id == orderId && o.UserId == userId);

            if (order == null)
            {
                return NotFound();
            }

            var pdfBytes = _pdfService.GenerateReceiptPdf(order);

            _logService.AddLog(
                "Document",
                $"Receipt PDF downloaded for OrderId: {order.Id}",
                userId,
                User.Identity?.Name
            );

            return File(
                pdfBytes,
                "application/pdf",
                $"Receipt_Order_{order.Id}.pdf"
            );
        }

        public IActionResult DownloadAgreementPdf(int orderId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var order = _context.Orders
                .Include(o => o.Items)
                .FirstOrDefault(o => o.Id == orderId && o.UserId == userId);

            if (order == null)
            {
                return NotFound();
            }

            var pdfBytes = _pdfService.GenerateAgreementPdf(order);

            _logService.AddLog(
                "Document",
                $"Agreement PDF downloaded for OrderId: {order.Id}",
                userId,
                User.Identity?.Name
            );

            return File(
                pdfBytes,
                "application/pdf",
                $"Agreement_Order_{order.Id}.pdf"
            );
        }
    }
}