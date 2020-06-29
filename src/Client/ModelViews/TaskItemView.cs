using NoCrast.Client.Utils;
using NoCrast.Shared.Model;
using System;

namespace NoCrast.Client.ModelViews
{
    public class TaskItemView
    {
        public TaskItem Task { get; }
        public TimeLogItem TimeLog { get; set; }
        public ITimeProvider Provider { get; set; }

        public TaskItemView(ITimeProvider provider, TaskItem task, TimeLogItem timeLog)
        {
            Provider = provider;
            Task = task;
            TimeLog = timeLog;
        }

        public long GetElapsedThisPeriod()
        {
            return (long)(Provider.CurrentTime - TimeLog.StartTime).TotalMilliseconds;
        }

        private long GetTotalMilliseconds()
        {
            long result = TimeLog.ElapsedMilliseconds;
            if (Task.IsRunning)
            {
                result += GetElapsedThisPeriod();
            }
            return result;
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
    }
}
