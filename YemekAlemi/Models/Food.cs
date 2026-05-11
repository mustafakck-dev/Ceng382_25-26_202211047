using System.ComponentModel.DataAnnotations;

namespace YemekAlemi.Models
{
    public class Food
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [Range(0.01, 10000)]
        public double Price { get; set; }

        [Required]
        public string Description { get; set; }
        public string? RestaurantName { get; set; }

        public string? Address { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }
        public string? ImageUrl { get; set; }
        public List<CustomizationOption> CustomizationOptions { get; set; }
    = new();
    }
}