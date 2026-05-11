namespace YemekAlemi.Models
{
    public class Log
    {
        public int Id { get; set; }

        public string Action { get; set; }

        public string UserEmail { get; set; }

        public string Description { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}