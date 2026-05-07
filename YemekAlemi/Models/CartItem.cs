namespace YemekAlemi.Models
{
    public class CartItem
    {
        public int FoodId { get; set; }

        public string Name { get; set; }

        public double Price { get; set; }

        public int Quantity { get; set; }

        public string Customization { get; set; }

        public double CustomizationPrice { get; set; }

        public double TotalPrice
        {
            get
            {
                return (Price + CustomizationPrice) * Quantity;
            }
        }
    }
}