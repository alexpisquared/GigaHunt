﻿using System;
using System.Collections.Generic;

namespace DB.QStats.Std.Models;

public partial class FirstnameRootObject
{
    public string Name { get; set; }

    public string NameSanitized { get; set; }

    public string Gender { get; set; }

    public int Samples { get; set; }

    public int Accuracy { get; set; }

    public string CountryOfOriginMapUrl { get; set; }

    public int CreditsUsed { get; set; }

    public string Duration { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? ChangedAt { get; set; }

    public virtual ICollection<FirstnameCountryXref> FirstnameCountryXrefs { get; set; } = new List<FirstnameCountryXref>();
}
