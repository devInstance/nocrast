using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace NoCrast.Server.Model
{
    public class TimeLog : DatabaseObject
    {
        public DateTime StartTime { get; set; }

        public long ElapsedMilliseconds { get; set; }

        [ForeignKey("Task")]
        public Guid TaskId { get; set; }
        public virtual TimerTask Task { get; set; }
    }

}
