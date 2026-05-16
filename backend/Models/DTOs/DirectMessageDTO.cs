namespace backend.Models.DTOs;

public class DirectMessageDTO
{
    public Guid Id { get; set; }
    public Guid SenderId { get; set; }
    public Guid RecipientId { get; set; }
    public string Body { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}
