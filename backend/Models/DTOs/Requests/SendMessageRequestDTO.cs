namespace backend.Models.DTOs.Requests;

public class SendMessageRequestDTO
{
    public Guid RecipientId { get; set; }
    public string Body { get; set; } = null!;
}
