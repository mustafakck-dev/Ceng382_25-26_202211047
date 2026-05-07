using System;

namespace YemekAlemi.Models
{
    public class AppLog
    {
        public int Id { get; set; }

        public string EventType { get; set; }

        public string Message { get; set; }

        public string? UserId { get; set; }

        public string? UserEmail { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}