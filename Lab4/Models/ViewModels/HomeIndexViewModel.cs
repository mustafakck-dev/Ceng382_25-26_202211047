using Lab4.Models.Identity;
using Lab4.Models.Media;

namespace Lab4.Models.ViewModels
{
    public class HomeIndexViewModel
    {
        public List<ApplicationUser> Users { get; set; } = new();
        public List<ImageItem> Images { get; set; } = new();
    }
}