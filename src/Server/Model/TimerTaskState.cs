using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace NoCrast.Server.Model
{
    public class TimerTaskState
    {
        [Key]
        public Guid Id { get; set; }
        public virtual TimerTask Task { get; set; }

        [Required]
        public bool IsRunning { get; set; }

        public virtual TimeLog? LatestTimeLogItem { get; set; }
    }
}
