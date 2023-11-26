using System;
using System.Collections.Generic;

namespace DB.QStats.Std.Models;

public partial class VwAgencyRate
{
    public string Agency { get; set; }

    public int? Cnt { get; set; }

    public int? MinRate { get; set; }

    public int? AvgRate { get; set; }

    public int? MaxRate { get; set; }

    public DateTime? LatestInstance { get; set; }
}
