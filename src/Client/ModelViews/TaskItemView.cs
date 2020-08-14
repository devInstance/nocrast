using NoCrast.Shared.Model;
using NoCrast.Shared.Utils;
using System;

namespace NoCrast.Client.ModelViews
{
    public class TaskItemView
    {
        public ITimeProvider Provider { get; set; }

        public TaskItem Task { get; set; }

        public TaskItemView(ITimeProvider provider, TaskItem task)
        {
            Provider = provider;
            Task = task;
        }

        public long GetElapsedThisPeriod()
        {
            if (Task.ActiveTimeLogItem != null)
            {
                return (long)(Provider.CurrentTime - Task.ActiveTimeLogItem.StartTime).TotalMilliseconds;
            }
            return 0;
        }

        private long GetTotalMilliseconds()
        {
            if (Task.ActiveTimeLogItem != null)
            {
                long result = Task.ActiveTimeLogItem.ElapsedMilliseconds;
                if (Task.IsRunning)
                {
                    result += GetElapsedThisPeriod();
                }
                return result;
            }
            return 0;
        }

        public TimeSpan GetElapsedTimeSpan()
        {
            return TimeSpan.FromMilliseconds(GetTotalMilliseconds());
        }

        public void Start()
        {
            TimeLogItem log = new TimeLogItem
            {
                StartTime = Provider.CurrentTime
            };

            Task.ActiveTimeLogItem = log;
            Task.IsRunning = true;
        }

        public void Stop()
        {
            Task.IsRunning = false;
            Task.ActiveTimeLogItem.ElapsedMilliseconds += GetElapsedThisPeriod();
        }

        public float TotalHoursSpent
        { 
            get
            {
                return (float)Task.TotalTimeSpent / (60 * 60 * 1000);
            }
        }

        public float TotalHoursSpentThisWeek
        {
            get
            {
                return (float)Task.TotalTimeSpentThisWeek / (60 * 60 * 1000);
            }
        }
        public float TotalHoursSpentToday
        {
            get
            {
                return (float)Task.TotalTimeSpentToday / (60 * 60 * 1000);
            }
        }
    }
}
