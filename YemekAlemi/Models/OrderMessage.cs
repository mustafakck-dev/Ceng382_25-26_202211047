using System.ComponentModel.DataAnnotations;

namespace YemekAlemi.Models
{
    public class OrderMessage
    {
        public int Id { get; set; }

        public int OrderId { get; set; }

        [Required]
        public string SenderEmail { get; set; } = "";

        [Required]
        public string ReceiverEmail { get; set; } = "";

        [Required]
        public string MessageText { get; set; } = "";

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public Order? Order { get; set; }
    }
}