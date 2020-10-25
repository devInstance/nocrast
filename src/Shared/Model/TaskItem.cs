using System;
using System.ComponentModel.DataAnnotations;

namespace NoCrast.Shared.Model
{
    public enum TaskFilter
    {
        All = 0,
        RunningOnly = 1,
        StoppedOnly = 2
    }

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

        public DateTime UpdateDate { get; set; }
    }
}
