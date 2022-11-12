using System;
using System.Collections.Generic;

namespace DB.QStats.Std.Models;

public partial class CvwatchLogView
{
    public DateTime? From { get; set; }

    public DateTime? Till { get; set; }

    public int? IdleHour { get; set; }

    public int? Exp { get; set; }

    public int? Doc { get; set; }

    public int? Txt { get; set; }

    public int? Msn { get; set; }
}
