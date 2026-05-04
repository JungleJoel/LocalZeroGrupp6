using System;
using System.Collections.Generic;

namespace backend.Models.Entities;

public partial class CommunityResident
{
    public Guid UserId { get; set; }

    public Guid CommunityId { get; set; }

    public bool IsManager { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Community Community { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
