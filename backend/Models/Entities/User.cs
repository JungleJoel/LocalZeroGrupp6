using System;
using System.Collections.Generic;

namespace backend.Models.Entities;

public partial class User
{
    public Guid Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string? AvatarImageUrl { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<CommunityJoinRequest> CommunityJoinRequestReviewedByNavigations { get; set; } = new List<CommunityJoinRequest>();

    public virtual ICollection<CommunityJoinRequest> CommunityJoinRequestUsers { get; set; } = new List<CommunityJoinRequest>();

    public virtual ICollection<CommunityResident> CommunityResidents { get; set; } = new List<CommunityResident>();

    public virtual ICollection<DirectMessage> DirectMessageRecipients { get; set; } = new List<DirectMessage>();

    public virtual ICollection<DirectMessage> DirectMessageSenders { get; set; } = new List<DirectMessage>();

    public virtual ICollection<EcoPointTransaction> EcoPointTransactions { get; set; } = new List<EcoPointTransaction>();

    public virtual ICollection<InitiativeCommentLike> InitiativeCommentLikes { get; set; } = new List<InitiativeCommentLike>();

    public virtual ICollection<InitiativeComment> InitiativeComments { get; set; } = new List<InitiativeComment>();

    public virtual ICollection<InitiativeLike> InitiativeLikes { get; set; } = new List<InitiativeLike>();

    public virtual ICollection<InitiativeParticipator> InitiativeParticipators { get; set; } = new List<InitiativeParticipator>();

    public virtual ICollection<InitiativeShare> InitiativeShares { get; set; } = new List<InitiativeShare>();

    public virtual ICollection<Initiative> Initiatives { get; set; } = new List<Initiative>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}
