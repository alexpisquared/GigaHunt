using System;
using System.Collections.Generic;

namespace DalEf5.Models
{
    public partial class OBSOLETE_ContactHistory___Use_EHist
    {
        public int ID { get; set; }
        public int ContactId { get; set; }
        public System.DateTime AddedAt { get; set; }
        public Nullable<System.DateTime> ISent { get; set; }
        public Nullable<System.DateTime> IReceived { get; set; }
        public string LetterSubject { get; set; }
        public string LetterBody { get; set; }
        public string Notes { get; set; }
        public virtual OBSOLETE_Contact___Use_EMail OBSOLETE_Contact___Use_EMail { get; set; }
    }
}
