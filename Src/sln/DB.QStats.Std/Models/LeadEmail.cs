﻿using System;
using System.Collections.Generic;

namespace DB.QStats.Std.Models;

public partial class LeadEmail
{
    public int Id { get; set; }

    public int LeadId { get; set; }

    public string EmailId { get; set; }

    public decimal? HourlyRate { get; set; }

    public virtual Email Email { get; set; }

    public virtual Lead Lead { get; set; }
}
