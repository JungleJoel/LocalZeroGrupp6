namespace backend.Models;

public record CommunityEcoActionSummaryDTO
(
    Guid CommunityId,
    int TotalActions,
    int TotalEcoPoints,
    int ActiveMembers,
    int ActionsThisMonth,
    string? TopActionReason,
    int TopActionCount
);
