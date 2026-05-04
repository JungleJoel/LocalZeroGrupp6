namespace backend.Models.DTOs;

public class CommunityMembershipDTO
{
    public Guid CommunityId { get; set; }
    public string CommunityName { get; set; } = string.Empty;
    public bool IsManager { get; set; }
}