namespace PaymentSystem.Models
{
    public class PaymentResponse
    {
        public string TransactionId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Timestamp { get; set; }
        public bool Success { get; set; }
    }
}
