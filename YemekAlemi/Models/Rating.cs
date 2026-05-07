using System.ComponentModel.DataAnnotations;

namespace YemekAlemi.Models
{
    public class Rating
    {
        public int Id { get; set; }

        public int OrderId { get; set; }

        public int FoodId { get; set; }

        public string UserId { get; set; }

        [Range(1, 5)]
        public int MenuRating { get; set; }

        [Range(1, 5)]
        public int CatererRating { get; set; }

        public string Comment { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}