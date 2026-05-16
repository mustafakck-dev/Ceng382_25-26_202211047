namespace YemekAlemi.Models
{
    public class Order
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public double TotalPrice { get; set; }

        public string Status { get; set; } // Pending / Completed

        public DateTime CreatedAt { get; set; }

        public List<OrderItem> Items { get; set; }
        public int GuestCount { get; set; }

        public string EventType { get; set; } = string.Empty;

        public DateTime? EventDate { get; set; }

        public string EventAddress { get; set; } = string.Empty;

        public string SpecialRequest { get; set; } = string.Empty;
    }
}