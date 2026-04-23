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
    }
}