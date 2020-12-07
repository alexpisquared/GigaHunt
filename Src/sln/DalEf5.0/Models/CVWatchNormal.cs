using System;
using System.Collections.Generic;

namespace DalEf5.Models
{
    public partial class CVWatchNormal
    {
        public int id { get; set; }
        public System.DateTime NewValueAt { get; set; }
        public System.DateTime LastSeenAt { get; set; }
        public Nullable<int> WrkPlsExp { get; set; }
        public Nullable<int> WrkPlsDoc { get; set; }
        public Nullable<int> WrkPlsTxt { get; set; }
        public Nullable<int> MsnMonster { get; set; }
        public string Notes { get; set; }
    }
}
