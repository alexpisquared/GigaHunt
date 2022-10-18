using System;
using System.Collections.Generic;

namespace DB.QStats.Std.Models
{
    public partial class CvwatchLogOld
    {
        public int Id { get; set; }
        public DateTime TimeObserved { get; set; }
        public int? WrkPlsExp { get; set; }
        public int? WrkPlsDoc { get; set; }
        public int? WrkPlsTxt { get; set; }
        public int? MsnMonster { get; set; }
        public string? Notes { get; set; }
        public DateTime? ObservedTime { get; set; }
        public DateTime? ObservedDate { get; set; }
        public int? RealMonster { get; set; }
        public string? Comment { get; set; }
    }
}
