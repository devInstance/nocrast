using System;
using System.ComponentModel.DataAnnotations;

namespace NoCrast.Shared.Model
{
    public class TaskItem
    {
        [Required]
        public string Id { get; set; }

        [Required]
        public string Title { get; set; }
 
        [Required]
        public bool IsRunning { get; set; }

        public string LatestTimeLogItemId { get; set; }

        public int TimeLogCount { get; set; }
    }
}
