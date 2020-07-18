using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace NoCrast.Server.Model
{
    public class TimerTask : DatabaseObject
    {
        public virtual UserProfile Profile { get; set; }

        [Required]
        public string Title { get; set; }
  
        public virtual ICollection<TimeLog> TimeLog { get; set; }

        [ForeignKey("State")]
        public Guid StateId { get; set; }
        public virtual TimerTaskState State { get; set; }
    }
}
