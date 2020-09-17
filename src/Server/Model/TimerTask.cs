using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NoCrast.Server.Model
{
    public class TimerTask : DatabaseObject
    {
        public virtual UserProfile Profile { get; set; }

        [Required]
        public string Title { get; set; }
  
        public virtual ICollection<TimeLog> TimeLog { get; set; }

        public virtual TimerTaskState State { get; set; }

        public virtual Project Project { get; set; }

        public string Descritpion { get; set; }
    }
}
