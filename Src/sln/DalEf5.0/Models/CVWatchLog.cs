using System;
using System.Collections.Generic;

namespace DalEf5.Models
{
    public partial class CVWatchLog
    {
        public int id { get; set; }
        public System.DateTime TimeObserved { get; set; }
        public Nullable<int> WrkPlsExp { get; set; }
        public Nullable<int> WrkPlsDoc { get; set; }
        public Nullable<int> WrkPlsTxt { get; set; }
        public Nullable<int> MsnMonster { get; set; }
        public string Notes { get; set; }
        public Nullable<System.DateTime> ObservedTime { get; set; }
        public Nullable<System.DateTime> ObservedDate { get; set; }
        public Nullable<int> RealMonster { get; set; }
        public string Comment { get; set; }
    }
}
