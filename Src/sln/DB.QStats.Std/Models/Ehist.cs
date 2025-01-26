using System;
using System.Collections.Generic;

namespace DB.QStats.Std.Models;

public partial class Ehist
{
    public int Id { get; set; }

    public string EmailId { get; set; }

    public string RecivedOrSent { get; set; }

    public DateTime? SentOn { get; set; }

    public DateTime EmailedAt { get; set; }

    public string? LetterSubject { get; set; }

    public string LetterBody { get; set; }

    public string Notes { get; set; }

    public DateTime AddedAt { get; set; }

    public virtual Email Email { get; set; }
}
