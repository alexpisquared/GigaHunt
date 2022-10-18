using System;
using System.Collections.Generic;

namespace DB.QStats.Std.Models
{
    public partial class ObsoleteContactHistoryUseEhist
    {
        public int Id { get; set; }
        public int ContactId { get; set; }
        public DateTime AddedAt { get; set; }
        public DateTime? Isent { get; set; }
        public DateTime? Ireceived { get; set; }
        public string? LetterSubject { get; set; }
        public string? LetterBody { get; set; }
        public string? Notes { get; set; }

        public virtual ObsoleteContactUseEmail Contact { get; set; } = null!;
    }
}
