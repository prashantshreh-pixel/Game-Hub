namespace GameHub.Domain;

public class Session
{
    public Guid Id { get; set; }
    public Guid StationId { get; set; }
    public Station Station { get; set; } = null!;
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public Guid? CustomerId { get; set; }
}
