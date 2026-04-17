using System;
using System.Collections.Generic;

namespace backend.Models.Entities;

public partial class InitiativeCommentLike
{
    public Guid CommentId { get; set; }

    public Guid UserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual InitiativeComment Comment { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
