using System;
using System.Collections.Generic;

namespace DB.QStats.Std.Models;

public partial class FirstnameCountryXref
{
    public int Id { get; set; }

    public float Probability { get; set; }

    public string Note { get; set; }

    public string Country { get; set; }

    public string Name { get; set; }

    public virtual CountryOfOrigin CountryNavigation { get; set; }

    public virtual FirstnameRootObject NameNavigation { get; set; }
}
