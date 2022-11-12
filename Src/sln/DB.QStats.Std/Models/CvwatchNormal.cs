using System;
using System.Collections.Generic;

namespace DB.QStats.Std.Models;

public partial class CvwatchNormal
{
    public int Id { get; set; }

    public DateTime NewValueAt { get; set; }

    public DateTime LastSeenAt { get; set; }

    public int? WrkPlsExp { get; set; }

    public int? WrkPlsDoc { get; set; }

    public int? WrkPlsTxt { get; set; }

    public int? MsnMonster { get; set; }

    public string? Notes { get; set; }
}
