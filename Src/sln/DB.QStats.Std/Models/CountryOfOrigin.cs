using System;
using System.Collections.Generic;

namespace DB.QStats.Std.Models;

public partial class CountryOfOrigin
{
    public string Country { get; set; } = null!;

    public string CountryName { get; set; } = null!;

    public float Probability { get; set; }

    public string ContinentalRegion { get; set; } = null!;

    public string StatisticalRegion { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? ChangedAt { get; set; }

    public string? FirstnameRootObjectname { get; set; }

    public virtual ICollection<FirstnameCountryXref> FirstnameCountryXrefs { get; } = new List<FirstnameCountryXref>();
}
