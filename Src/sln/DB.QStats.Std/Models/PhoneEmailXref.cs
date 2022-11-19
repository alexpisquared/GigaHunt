using System;
using System.Collections.Generic;

namespace DB.QStats.Std.Models;

public partial class PhoneEmailXref
{
    public int Id { get; set; }

    public int PhoneId { get; set; }

    public string EmailId { get; set; } = null!;

    public string Note { get; set; } = null!;

    public DateTime AddedAt { get; set; }

    public virtual Email Email { get; set; } = null!;

    public virtual Phone Phone { get; set; } = null!;
}
