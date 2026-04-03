using System.Collections.Generic;

namespace Northwind.Mvc.Models
{
    public class ShipperContactInfoPageViewModel
    {
        public List<ShipperContactInfo> ActiveItems { get; set; } = new List<ShipperContactInfo>();
        public List<ShipperContactInfo> DeletedItems { get; set; } = new List<ShipperContactInfo>();

        public ShipperContactInfo? EditItem { get; set; }
    }
}