namespace backend.Models.DTOs.Responses
{
    public record GetMyCommunityResponseDTO
    (
        CommunityDTO Community,
        bool IsCommunityManager
        // add initiatives here
    );
}
