using System;
using System.Collections.Generic;

namespace DB.QStats.Std.Models;

public partial class Agency
{
    public string Id { get; set; } = null!;

    public int? TtlAgents { get; set; }

    public string? Address { get; set; }

    public bool IsBroadcastee { get; set; }

    public string? Note { get; set; }

    public DateTime AddedAt { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public virtual ICollection<Email> Emails { get; } = new List<Email>();

    public virtual ICollection<PhoneAgencyXref> PhoneAgencyXrefs { get; } = new List<PhoneAgencyXref>();
}
