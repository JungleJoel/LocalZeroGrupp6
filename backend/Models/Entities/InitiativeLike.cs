using System;
using System.Collections.Generic;

namespace backend.Models.Entities;

public partial class InitiativeLike
{
    public Guid InitiativeId { get; set; }

    public Guid UserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Initiative Initiative { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
