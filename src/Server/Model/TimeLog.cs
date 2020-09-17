using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace NoCrast.Server.Model
{
    public class TimeLog : DatabaseObject
    {
        public DateTime StartTime { get; set; }

        public long ElapsedMilliseconds { get; set; }

        public string Notes { get; set; }

        [ForeignKey("Task")]
        public Guid TaskId { get; set; }
        public virtual TimerTask Task { get; set; }
    }
}
