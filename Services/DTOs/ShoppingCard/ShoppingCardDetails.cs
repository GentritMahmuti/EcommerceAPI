namespace Services.DTOs.ShoppingCard
{
    public class ShoppingCardDetails
    {
        public double CardTotal { get; set; }

        public List<ShoppingCardViewDto> ShoppingCardItems { get; set; }
        public int ItemCount { get; set; }
    }
}
