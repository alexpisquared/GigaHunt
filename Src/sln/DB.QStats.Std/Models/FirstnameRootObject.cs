using System;
using System.Collections.Generic;

namespace DB.QStats.Std.Models;

public partial class FirstnameRootObject
{
    public string Name { get; set; } = null!;

    public string NameSanitized { get; set; } = null!;

    public string Gender { get; set; } = null!;

    public int Samples { get; set; }

    public int Accuracy { get; set; }

    public string CountryOfOriginMapUrl { get; set; } = null!;

    public int CreditsUsed { get; set; }

    public string Duration { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? ChangedAt { get; set; }

    public virtual ICollection<FirstnameCountryXref> FirstnameCountryXrefs { get; } = new List<FirstnameCountryXref>();
}
