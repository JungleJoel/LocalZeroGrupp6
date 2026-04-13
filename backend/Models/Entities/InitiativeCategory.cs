using System;
using System.Collections.Generic;

namespace backend.Models.Entities;

public partial class InitiativeCategory
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string? IconName { get; set; }

    public virtual ICollection<Initiative> Initiatives { get; set; } = new List<Initiative>();
}
