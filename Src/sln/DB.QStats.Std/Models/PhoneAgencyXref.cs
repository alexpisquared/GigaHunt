using System;
using System.Collections.Generic;

namespace DB.QStats.Std.Models;

public partial class PhoneAgencyXref
{
    public int Id { get; set; }

    public int PhoneId { get; set; }

    public string AgencyId { get; set; }

    public string Note { get; set; }

    public DateTime AddedAt { get; set; }

    public virtual Agency Agency { get; set; }

    public virtual Phone Phone { get; set; }
}
