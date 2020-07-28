﻿using System;
using System.ComponentModel.DataAnnotations;

namespace NoCrast.Shared.Model
{
    public class TaskItem : ModelItem
    {
        [Required]
        public string Title { get; set; }
 
        [Required]
        public bool IsRunning { get; set; }

        public string ActiveTimeLogItemId { get; set; }

        public int TimeLogCount { get; set; }

        public long TotalTimeSpent { get; set; }

        public long TotalTimeSpentThisWeek { get; set; }

        public long TotalTimeSpentToday { get; set; }
    }
}
