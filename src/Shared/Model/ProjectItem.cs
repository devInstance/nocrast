using System.ComponentModel.DataAnnotations;

namespace NoCrast.Shared.Model
{
    public enum ProjectColor
    {
        Aqua,
        Blue,
        Darkblue,
        Green,
        Yellow,
        Orange,
        Red,
        Rose,
        Purple,
    }
    public class ProjectItem : ModelItem
    {
        [Required]
        public string Title { get; set; }

        public string Descritpion { get; set; }

        public ProjectColor Color { get; set; }
        public float Rate { get; set; }

        public int TasksCount { get; set; }

        public long TotalTimeSpent { get; set; }
    }
}
