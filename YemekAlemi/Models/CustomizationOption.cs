using System.ComponentModel.DataAnnotations;

namespace YemekAlemi.Models
{
    public class CustomizationOption
    {
        public int Id { get; set; }

        public int FoodId { get; set; }

        public Food? Food { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public decimal ExtraPrice { get; set; }
    }
}