namespace backend.Models.DTOs;

public class ConversationSummaryDTO
{
    public Guid UserId { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? AvatarImageUrl { get; set; }
    public string LastMessage { get; set; } = null!;
    public DateTime LastMessageAt { get; set; }
}
