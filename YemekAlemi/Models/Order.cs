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
    }
}