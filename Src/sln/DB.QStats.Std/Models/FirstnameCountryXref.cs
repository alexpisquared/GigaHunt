using System;
using System.Collections.Generic;

namespace DB.QStats.Std.Models
{
    public partial class FirstnameCountryXref
    {
        public int Id { get; set; }
        public float Probability { get; set; }
        public string Note { get; set; } = null!;
        public string Country { get; set; } = null!;
        public string Name { get; set; } = null!;

        public virtual CountryOfOrigin CountryNavigation { get; set; } = null!;
        public virtual FirstnameRootObject NameNavigation { get; set; } = null!;
    }
}
