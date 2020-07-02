using System;
using System.ComponentModel.DataAnnotations;

namespace NoCrast.Shared.Model
{
    public class TaskItem : ModelItem
    {
        [Required]
        public string Title { get; set; }
 
        [Required]
        public bool IsRunning { get; set; }

        public string LatestTimeLogItemId { get; set; }

        public int TimeLogCount { get; set; }

        public long TotalTimeSpent { get; set; }
    }
}
