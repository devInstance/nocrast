using System.ComponentModel.DataAnnotations;

namespace NoCrast.Shared.Model
{
    public class TaskItem : ModelItem
    {
        [Required]
        public string Title { get; set; }
 
        [Required]
        public bool IsRunning { get; set; }

        public string Descritpion { get; set; }

        [Required]
        public ProjectItem Project { get; set; }

        public TimeLogItem ActiveTimeLogItem { get; set; }

        public int TimeLogCount { get; set; }

        public long TotalTimeSpent { get; set; }

        public long TotalTimeSpentThisWeek { get; set; }

        public long TotalTimeSpentToday { get; set; }
    }
}
