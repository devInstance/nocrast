using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NoCrast.Server.Model
{
    public class TimerTag : DatabaseObject
    {
        public virtual UserProfile Profile { get; set; }

        [Required]
        public string Name { get; set; }
    }
}
