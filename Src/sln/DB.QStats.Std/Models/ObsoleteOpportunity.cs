using System;
using System.Collections.Generic;

namespace DB.QStats.Std.Models;

public partial class ObsoleteOpportunity
{
    public int Id { get; set; }

    public DateTime AddedAt { get; set; }

    public DateTime LastActivityAt { get; set; }

    public string Company { get; set; } = null!;

    public string Location { get; set; } = null!;

    public int? ContactId { get; set; }

    public string Description { get; set; } = null!;

    public int RateAsked { get; set; }

    public string Start { get; set; } = null!;

    public string Term { get; set; } = null!;

    public virtual ObsoleteContactUseEmail? Contact { get; set; }
}
