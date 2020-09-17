using System.ComponentModel.DataAnnotations;

namespace NoCrast.Shared.Model
{
    public class ProjectItem : ModelItem
    {
        [Required]
        public string Title { get; set; }

        public string Descritpion { get; set; }
    }
}
