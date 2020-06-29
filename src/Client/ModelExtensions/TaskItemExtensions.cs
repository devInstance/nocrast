using NoCrast.Client.Utils;
using NoCrast.Shared.Model;
using System;

namespace NoCrast.Client.ModelExtensions
{
    public static class TaskItemExtensions
    {
        //public static long GetElapsedThisPeriod(this TaskItem item, ITimeProvider provider)
        //{
        //    return (long)(provider.CurrentTime - item.LastStartTime).TotalMilliseconds;
        //}

        //private static long GetTotalMilliseconds(this TaskItem item, ITimeProvider provider)
        //{
        //    long result = item.ElapsedMilliseconds;
        //    if (item.IsRunning)
        //    {
        //        result += item.GetElapsedThisPeriod(provider);
        //    }
        //    return result;
        //}

        //public static TimeSpan GetElapsedTimeSpan(this TaskItem item, ITimeProvider provider)
        //{
        //    return TimeSpan.FromMilliseconds(item.GetTotalMilliseconds(provider));
        //}

        //public static void Start(this TaskItem item, ITimeProvider provider)
        //{
        //    item.LastStartTime = provider.CurrentTime;
        //    item.IsRunning = true;
        //}

        //public static void Stop(this TaskItem item, ITimeProvider provider)
        //{
        //    item.IsRunning = false;
        //    item.ElapsedMilliseconds += item.GetElapsedThisPeriod(provider);
        //}
    }
}
