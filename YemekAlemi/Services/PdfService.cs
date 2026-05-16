using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using YemekAlemi.Models;

namespace YemekAlemi.Services
{
    public class PdfService
    {
        public byte[] GenerateReceiptPdf(Order order)
        {
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(40);
                    page.Size(PageSizes.A4);
                    page.DefaultTextStyle(x => x.FontSize(11));

                    page.Header()
                        .Text("YemekAlemi Catering Receipt")
                        .SemiBold()
                        .FontSize(24)
                        .FontColor(Colors.Green.Darken3);

                    page.Content().Column(column =>
                    {
                        column.Spacing(12);

                        column.Item().Text($"Receipt No: RCPT-{order.Id}");
                        column.Item().Text($"Order ID: {order.Id}");
                        column.Item().Text($"Status: {order.Status}");
                        column.Item().Text($"Purchase Time: {order.CreatedAt}");
                        column.Item().Text($"Guest Count: {order.GuestCount}");
                        column.Item().Text($"Event Type: {order.EventType}");
                        column.Item().Text($"Event Date: {order.EventDate}");
                        column.Item().Text($"Event Address: {order.EventAddress}");
                        column.Item().Text($"Special Request: {order.SpecialRequest}");

                        column.Item().LineHorizontal(1);

                        column.Item().Text("Selected Catering Packages")
                            .SemiBold()
                            .FontSize(16);

                        foreach (var item in order.Items)
                        {
                            var perGuestPrice = item.Price + item.CustomizationPrice;
                            var lineTotal = perGuestPrice * order.GuestCount;

                            column.Item().Text(
                                $"{item.Name} | Customization: {item.Customization} | Per Guest: {perGuestPrice} ₺ | Total: {lineTotal} ₺"
                            );
                        }

                        column.Item().LineHorizontal(1);

                        column.Item().Text($"Total Catering Cost: {order.TotalPrice} ₺")
                            .SemiBold()
                            .FontSize(18);
                    });

                    page.Footer()
                        .AlignCenter()
                        .Text("This receipt was generated dynamically by YemekAlemi Catering Platform.");
                });
            }).GeneratePdf();
        }

        public byte[] GenerateAgreementPdf(Order order)
        {
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(40);
                    page.Size(PageSizes.A4);
                    page.DefaultTextStyle(x => x.FontSize(11));

                    page.Header()
                        .Text("YemekAlemi Catering Service Agreement")
                        .SemiBold()
                        .FontSize(24)
                        .FontColor(Colors.Green.Darken3);

                    page.Content().Column(column =>
                    {
                        column.Spacing(12);

                        column.Item().Text(
                            "This agreement confirms that the customer has completed a catering booking through the YemekAlemi Catering Management Platform."
                        );

                        column.Item().LineHorizontal(1);

                        column.Item().Text("Agreement Information")
                            .SemiBold()
                            .FontSize(16);

                        column.Item().Text($"Agreement No: AGR-{order.Id}");
                        column.Item().Text($"Order ID: {order.Id}");
                        column.Item().Text($"Customer/User ID: {order.UserId}");
                        column.Item().Text($"Agreement Status: {order.Status}");
                        column.Item().Text($"Agreement Date: {order.CreatedAt}");

                        column.Item().LineHorizontal(1);

                        column.Item().Text("Catering Event Information")
                            .SemiBold()
                            .FontSize(16);

                        column.Item().Text($"Event Type: {order.EventType}");
                        column.Item().Text($"Guest Count: {order.GuestCount}");
                        column.Item().Text($"Event Date: {order.EventDate}");
                        column.Item().Text($"Event Address: {order.EventAddress}");
                        column.Item().Text($"Special Request: {order.SpecialRequest}");

                        column.Item().LineHorizontal(1);

                        column.Item().Text("Included Catering Packages")
                            .SemiBold()
                            .FontSize(16);

                        foreach (var item in order.Items)
                        {
                            var perGuestPrice = item.Price + item.CustomizationPrice;
                            var packageTotal = perGuestPrice * order.GuestCount;

                            column.Item().Text(
                                $"{item.Name} | Customization: {item.Customization} | Per Guest: {perGuestPrice} ₺ | Package Total: {packageTotal} ₺"
                            );
                        }

                        column.Item().LineHorizontal(1);

                        column.Item().Text($"Total Agreement Amount: {order.TotalPrice} ₺")
                            .SemiBold()
                            .FontSize(18);

                        column.Item().Text(
                            "Terms: The customer confirms that the provided event information is accurate. Catering preparation is planned according to event date, guest count and selected packages."
                        );
                    });

                    page.Footer()
                        .AlignCenter()
                        .Text("This agreement was generated dynamically by YemekAlemi Catering Platform.");
                });
            }).GeneratePdf();
        }
    }
}