using System;
using System.Collections.Generic;

namespace DB.QStats.Std.Models;

public partial class ObsoleteOpportunity
{
    public int Id { get; set; }

    public DateTime AddedAt { get; set; }

    public DateTime LastActivityAt { get; set; }

    public string Company { get; set; }

    public string Location { get; set; }

    public int? ContactId { get; set; }

    public string Description { get; set; }

    public int RateAsked { get; set; }

    public string Start { get; set; }

    public string Term { get; set; }

    public virtual ObsoleteContactUseEmail Contact { get; set; }
}
