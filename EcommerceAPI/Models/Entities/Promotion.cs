namespace EcommerceAPI.Models.Entities
{
    public class Promotion
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public double DiscountAmount { get; set; }
        //Check if promo code is valid or not
        public bool IsActive()
        {
            return StartDate <= DateTime.Now && EndDate >= DateTime.Now;
        }
        public ICollection<OrderData> OrderDatas { get; set; }
    }
}
