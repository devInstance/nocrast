using System.ComponentModel.DataAnnotations;

namespace NoCrast.Server.Model
{
    public class TimerTag : DatabaseObject
    {
        public virtual UserProfile Profile { get; set; }

        [Required]
        public string Name { get; set; }

        public int Color { get; set; }
    }
}
