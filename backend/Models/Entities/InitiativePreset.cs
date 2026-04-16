using System;
using System.Collections.Generic;

namespace backend.Models.Entities;

public partial class InitiativePreset
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public int? EcoPointsPerParticipant { get; set; }

    public virtual ICollection<Initiative> Initiatives { get; set; } = new List<Initiative>();
}
