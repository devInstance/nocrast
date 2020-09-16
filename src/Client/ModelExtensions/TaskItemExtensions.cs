using NoCrast.Shared.Model;
using NoCrast.Shared.Utils;
using System;

namespace NoCrast.Client.ModelExtensions
{
    public static class TaskItemExtensions
    {
        public static long GetElapsedThisPeriod(this TaskItem item, ITimeProvider provider)
        {
            if (item.ActiveTimeLogItem != null)
            {
                return (long)(provider.CurrentTime - item.ActiveTimeLogItem.StartTime).TotalMilliseconds;
            }
            return 0;
        }

        private static long GetTotalMilliseconds(this TaskItem item, ITimeProvider provider)
        {
            if (item.ActiveTimeLogItem != null)
            {
                long result = item.ActiveTimeLogItem.ElapsedMilliseconds;
                if (item.IsRunning)
                {
                    result += item.GetElapsedThisPeriod(provider);
                }
                return result;
            }
            return 0;
        }

        public static TimeSpan GetElapsedTimeSpan(this TaskItem item, ITimeProvider provider)
        {
            return TimeSpan.FromMilliseconds(item.GetTotalMilliseconds(provider));
        }

        public static void Start(this TaskItem item, ITimeProvider provider)
        {
            TimeLogItem log = new TimeLogItem
            {
                StartTime = provider.CurrentTime
            };

            item.ActiveTimeLogItem = log;
            item.IsRunning = true;
        }

        public static void Stop(this TaskItem item, ITimeProvider provider)
        {
            item.IsRunning = false;
            item.ActiveTimeLogItem.ElapsedMilliseconds += item.GetElapsedThisPeriod(provider);
        }

        public static float GetTotalHoursSpent(this TaskItem item)
        {
            return (float)item.TotalTimeSpent / (60 * 60 * 1000);
        }

        public static float GetTotalHoursSpentThisWeek(this TaskItem item)
        {
            return (float)item.TotalTimeSpentThisWeek / (60 * 60 * 1000);
        }
        public static float GetTotalHoursSpentToday(this TaskItem item)
        {
            return (float)item.TotalTimeSpentToday / (60 * 60 * 1000);
        }

        public static float GetTotalHoursSpentTodayTillNow(this TaskItem item, ITimeProvider provider)
        {
            var result = item.GetTotalHoursSpentToday();
            if (item.IsRunning)
            {
                return result + ((float)item.GetElapsedThisPeriod(provider) / (60 * 60 * 1000));
            }
            return result;
        }
    }
}
