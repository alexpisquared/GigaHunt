using System;
using System.Collections.Generic;

namespace DalEf5.Models
{
    public partial class CVOnlineUpdateTime
    {
        public int Id { get; set; }
        public System.DateTime UpdatedAt { get; set; }
        public string Notes { get; set; }
    }
}
