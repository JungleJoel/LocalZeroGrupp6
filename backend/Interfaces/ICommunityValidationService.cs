namespace backend.Interfaces;

public interface ICommunityValidationService
{
    Task<bool> IsResidentInCommunityAsync(Guid communityId, Guid userId);
}