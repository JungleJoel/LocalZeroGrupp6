using System;
using System.Collections.Generic;

namespace backend.Models.Entities;

public partial class InitiativeShare
{
    public Guid InitiativeId { get; set; }

    public Guid TargetCommunityId { get; set; }

    public Guid SharedByUserId { get; set; }

    public DateTime SharedAt { get; set; }

    public virtual Initiative Initiative { get; set; } = null!;

    public virtual User SharedByUser { get; set; } = null!;

    public virtual Community TargetCommunity { get; set; } = null!;
}
