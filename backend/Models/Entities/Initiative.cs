using System;
using System.Collections.Generic;

namespace backend.Models.Entities;

public partial class Initiative
{
    public Guid Id { get; set; }

    public Guid CommunityId { get; set; }

    public Guid? CreatedBy { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public Guid? CategoryId { get; set; }

    public Guid? PresetId { get; set; }

    public bool IsPublic { get; set; }

    public double Latitude { get; set; }

    public double Longitude { get; set; }

    public DateTime StartsAt { get; set; }

    public DateTime? EstimatedEndsAt { get; set; }

    public DateTime? EndedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public int? EcoPointsPerParticipant { get; set; }

    public virtual InitiativeCategory? Category { get; set; }

    public virtual Community Community { get; set; } = null!;

    public virtual User? CreatedByNavigation { get; set; }

    public virtual ICollection<EcoPointTransaction> EcoPointTransactions { get; set; } = new List<EcoPointTransaction>();

    public virtual ICollection<InitiativeComment> InitiativeComments { get; set; } = new List<InitiativeComment>();

    public virtual ICollection<InitiativeLike> InitiativeLikes { get; set; } = new List<InitiativeLike>();

    public virtual ICollection<InitiativeParticipator> InitiativeParticipators { get; set; } = new List<InitiativeParticipator>();

    public virtual ICollection<InitiativeShare> InitiativeShares { get; set; } = new List<InitiativeShare>();

    public virtual InitiativePreset? Preset { get; set; }
}
