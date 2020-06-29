using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace NoCrast.Shared.Model
{
    public class TimeLogItem
    {
        [Required]
        public string Id { get; set; }

        public DateTime StartTime { get; set; }

        public long ElapsedMilliseconds { get; set; }

    }
}
