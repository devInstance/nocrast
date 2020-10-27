using NoCrast.Shared.Model;
using System.ComponentModel.DataAnnotations;

namespace NoCrast.Server.Model
{
    public class Project : DatabaseObject
    {
        public virtual UserProfile Profile { get; set; }

        [Required]
        public string Title { get; set; }

        public string Descritpion { get; set; }

        public ProjectColor Color { get; set; }
    }
}
