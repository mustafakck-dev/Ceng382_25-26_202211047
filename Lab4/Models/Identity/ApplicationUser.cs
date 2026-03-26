using Microsoft.AspNetCore.Identity;

namespace Lab4.Models.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string? Address { get; set; }
        public byte[]? Photo { get; set; }
        public string? PhotoContentType { get; set; }
    }
}