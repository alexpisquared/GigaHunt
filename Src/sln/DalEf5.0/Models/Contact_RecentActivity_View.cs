using System;
using System.Collections.Generic;

namespace DalEf5.Models
{
    public partial class Contact_RecentActivity_View
    {
        public System.DateTime AddedAt { get; set; }
        public string FName { get; set; }
        public string LName { get; set; }
        public string EMail { get; set; }
        public Nullable<System.DateTime> LastSent { get; set; }
        public Nullable<System.DateTime> LastRcvd { get; set; }
        public string LastSubject { get; set; }
        public string Notes { get; set; }
    }
}
