using System;
using System.Collections.Generic;

namespace backend.Models.Entities;

public partial class DirectMessage
{
    public Guid Id { get; set; }

    public Guid SenderId { get; set; }

    public Guid RecipientId { get; set; }

    public string Body { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual User Recipient { get; set; } = null!;

    public virtual User Sender { get; set; } = null!;
}
