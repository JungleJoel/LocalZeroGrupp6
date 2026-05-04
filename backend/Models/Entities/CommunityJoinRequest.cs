using System;
using System.Collections.Generic;

namespace backend.Models.Entities;

public partial class CommunityJoinRequest
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid CommunityId { get; set; }

    public Guid? ReviewedBy { get; set; }

    public bool? IsAccepted { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Community Community { get; set; } = null!;

    public virtual User? ReviewedByNavigation { get; set; }

    public virtual User User { get; set; } = null!;
}
