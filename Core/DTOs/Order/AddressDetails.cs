using System.ComponentModel.DataAnnotations;

namespace Services.DTOs.Order
{
    public class AddressDetails
    {
        public string PhoheNumber { get; set; }
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }
}
