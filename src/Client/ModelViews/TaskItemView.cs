using NoCrast.Shared.Model;
using NoCrast.Shared.Utils;
using System;

namespace NoCrast.Client.ModelViews
{
    public class TaskItemView
    {
        public ITimeProvider Provider { get; set; }

        public TaskItem Task { get; set; }
        public TimeLogItem ActiveTimeLog { get; set; }

        public TaskItemView(ITimeProvider provider, TaskItem task, TimeLogItem timeLog)
        {
            Provider = provider;
            Task = task;
            ActiveTimeLog = timeLog;
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
