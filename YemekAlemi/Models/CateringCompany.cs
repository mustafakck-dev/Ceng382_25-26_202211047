namespace YemekAlemi.Models
{
    public class CateringCompany
    {
        public int Id { get; set; }

        public string CompanyName { get; set; } = "";

        public string OwnerEmail { get; set; } = "";

        public string Address { get; set; } = "";

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public string Description { get; set; } = "";

        public string? ImageUrl { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}