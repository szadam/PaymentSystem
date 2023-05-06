
namespace PaymentSystem.Models
{
    public class PaymentRequest
    {
        public decimal Amount { get; set; }
        public string CreditCardNumber { get; set; }
        public string ExpirationDate { get; set; }
        public string CVV { get; set; }
        public string CardholderName { get; set; }
        public int TripId { get; set; }
        public decimal TripPrice { get; set; }
    }
}
