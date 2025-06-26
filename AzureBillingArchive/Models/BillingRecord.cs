public class BillingRecord
{
    public string Id { get; set; }
    public string PartitionKey { get; set; }
    public DateTime Timestamp { get; set; }
    public string CustomerId { get; set; }
    public decimal Amount { get; set; }
    // Add other relevant fields
}