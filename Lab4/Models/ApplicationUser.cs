using Microsoft.AspNetCore.Identity;

namespace Lab4.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string? Address { get; set; }
        public byte[]? Photo { get; set; }
        public string? PhotoContentType { get; set; }
       

    }
}