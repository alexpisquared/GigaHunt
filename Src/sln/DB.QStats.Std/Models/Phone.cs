using System;
using System.Collections.Generic;

namespace DB.QStats.Std.Models;

public partial class Phone
{
    public int Id { get; set; }

    public string PhoneNumber { get; set; } = null!;

    public DateTime SeenFirst { get; set; }

    public DateTime SeenLast { get; set; }

    public string? Notes { get; set; }

    public DateTime AddedAt { get; set; }

    public virtual ICollection<PhoneAgencyXref> PhoneAgencyXrefs { get; } = new List<PhoneAgencyXref>();

    public virtual ICollection<PhoneEmailXref> PhoneEmailXrefs { get; } = new List<PhoneEmailXref>();
}
