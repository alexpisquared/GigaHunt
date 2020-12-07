using System;
using System.Collections.Generic;

namespace DalEf5.Models
{
    public partial class EHist
    {
        public int ID { get; set; }
        public string EMailID { get; set; }
        public string RecivedOrSent { get; set; }
        public System.DateTime EmailedAt { get; set; }
        public string LetterSubject { get; set; }
        public string LetterBody { get; set; }
        public string Notes { get; set; }
        public System.DateTime AddedAt { get; set; }
        public virtual EMail EMail { get; set; }
    }
}
