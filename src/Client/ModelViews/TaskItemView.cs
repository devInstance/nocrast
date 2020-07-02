using NoCrast.Client.Utils;
using NoCrast.Shared.Model;
using System;

namespace NoCrast.Client.ModelViews
{
    public class TaskItemView
    {
        public ITimeProvider Provider { get; set; }

        public TaskItem Task { get; }
        public TimeLogItem TimeLog { get; set; }

        public long TotalTimeSpent { get; set; }

        public TaskItemView(ITimeProvider provider, TaskItem task, TimeLogItem timeLog, long totalTimeSpent)
        {
            Provider = provider;
            Task = task;
            TimeLog = timeLog;
            TotalTimeSpent = totalTimeSpent;
        }

        public long GetElapsedThisPeriod()
        {
            if (TimeLog != null)
            {
                return (long)(Provider.CurrentTime - TimeLog.StartTime).TotalMilliseconds;
            }
            return 0;
        }

        private long GetTotalMilliseconds()
        {
            if (TimeLog != null)
            {
                long result = TimeLog.ElapsedMilliseconds;
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
            TimeLog.StartTime = Provider.CurrentTime;
            Task.IsRunning = true;
        }

        public void Stop()
        {
            Task.IsRunning = false;
            TimeLog.ElapsedMilliseconds += GetElapsedThisPeriod();
        }

        public float TotalHoursSpent
        { 
            get
            {
                return (float)TotalTimeSpent / (60 * 60 * 1000);
            }
        }
    }
}
