namespace GameHub.Domain;

public class Order
{
    public Guid Id { get; set; }
    public Guid SessionId { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAt { get; set; }
}
