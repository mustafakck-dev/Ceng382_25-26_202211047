namespace YemekAlemi.Models
{
    public class EmailLog
    {
        public int Id { get; set; }

        public string ToEmail { get; set; } = string.Empty;

        public string Subject { get; set; } = string.Empty;

        public string Body { get; set; } = string.Empty;

        public DateTime SentAt { get; set; }
    }
}