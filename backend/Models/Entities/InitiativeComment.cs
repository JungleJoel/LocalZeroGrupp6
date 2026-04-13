using System;
using System.Collections.Generic;

namespace backend.Models.Entities;

public partial class InitiativeComment
{
    public Guid Id { get; set; }

    public Guid InitiativeId { get; set; }

    public Guid UserId { get; set; }

    public string Body { get; set; } = null!;

    public string? ImageUrl { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Initiative Initiative { get; set; } = null!;

    public virtual ICollection<InitiativeCommentLike> InitiativeCommentLikes { get; set; } = new List<InitiativeCommentLike>();

    public virtual User User { get; set; } = null!;
}
