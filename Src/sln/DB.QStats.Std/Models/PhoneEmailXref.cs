﻿using System;
using System.Collections.Generic;

namespace DB.QStats.Std.Models;

public partial class PhoneEmailXref
{
    public int Id { get; set; }

    public int PhoneId { get; set; }

    public string EmailId { get; set; }

    public string Note { get; set; }

    public DateTime AddedAt { get; set; }

    public virtual Email Email { get; set; }

    public virtual Phone Phone { get; set; }
}
