using System;

namespace NoCrast.Shared.Model
{
    public class TaskItem
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public bool IsRunning { get; set; }
        public DateTime LastStartTime { get; set; }
        public long ElapsedMilliseconds { get; set; }
    }
}
