using System;
using System.Collections.Generic;

namespace backend.Models.Entities;

public partial class Community
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public double? Latitude { get; set; }

    public double? Longitude { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<CommunityJoinRequest> CommunityJoinRequests { get; set; } = new List<CommunityJoinRequest>();

    public virtual ICollection<CommunityResident> CommunityResidents { get; set; } = new List<CommunityResident>();

    public virtual ICollection<EcoPointTransaction> EcoPointTransactions { get; set; } = new List<EcoPointTransaction>();

    public virtual ICollection<InitiativeShare> InitiativeShares { get; set; } = new List<InitiativeShare>();

    public virtual ICollection<Initiative> Initiatives { get; set; } = new List<Initiative>();
}
