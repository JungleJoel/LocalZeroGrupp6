using System;
using System.Collections.Generic;

namespace backend.Models.Entities;

public partial class EcoPointTransaction
{
    public Guid Id { get; set; }

    public Guid CommunityId { get; set; }

    public Guid UserId { get; set; }

    public Guid? InitiativeId { get; set; }

    public int Amount { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Community Community { get; set; } = null!;

    public virtual Initiative? Initiative { get; set; }

    public virtual User User { get; set; } = null!;
}
