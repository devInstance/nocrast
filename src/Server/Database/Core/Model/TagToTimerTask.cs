using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace NoCrast.Server.Model
{
    public class TagToTimerTask : InternalDatabaseObject
    {
        [ForeignKey("Tag")]
        public Guid TagId { get; set; }
        public virtual TimerTag Tag { get; set; }
        [ForeignKey("Task")]
        public Guid TaskId { get; set; }
        public virtual TimerTask Task { get; set; }
    }
}
