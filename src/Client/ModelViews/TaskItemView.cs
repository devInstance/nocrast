using NoCrast.Shared.Model;
using NoCrast.Shared.Utils;
using System;

namespace NoCrast.Client.ModelViews
{
    public class TaskItemView
    {
        public ITimeProvider Provider { get; set; }

        public TaskItem Task { get; }
        public TimeLogItem ActiveTimeLog { get; set; }
        public long TotalTimeSpent { get; set; }

        public TaskItemView(ITimeProvider provider, TaskItem task, TimeLogItem timeLog, long totalTimeSpent)
        {
            Provider = provider;
            Task = task;
            ActiveTimeLog = timeLog;
            TotalTimeSpent = totalTimeSpent;
        }

        public long GetElapsedThisPeriod()
        {
            if (ActiveTimeLog != null)
            {
                return (long)(Provider.CurrentTime - ActiveTimeLog.StartTime).TotalMilliseconds;
            }
            return 0;
        }

        private long GetTotalMilliseconds()
        {
            if (ActiveTimeLog != null)
            {
                long result = ActiveTimeLog.ElapsedMilliseconds;
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
            ActiveTimeLog.StartTime = Provider.CurrentTime;
            Task.IsRunning = true;
        }

        public void Stop()
        {
            Task.IsRunning = false;
            ActiveTimeLog.ElapsedMilliseconds += GetElapsedThisPeriod();
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
