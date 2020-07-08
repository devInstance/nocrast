﻿using NoCrast.Client.Utils;
using NoCrast.Shared.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoCrast.Client.ModelExtensions
{
    public static class TimeLogItemExtensions
    {
        public static long GetElapsedThisPeriod(this TimeLogItem logItem, ITimeProvider provider)
        {
            if (logItem != null)
            {
                if (logItem.ElapsedMilliseconds > 0)
                {
                    return logItem.ElapsedMilliseconds;
                }
                return (long)(provider.CurrentTime - logItem.StartTime).TotalMilliseconds;
            }
            return 0;
        }

        public static TimeSpan GetElapsedTimeSpan(this TimeLogItem logItem, ITimeProvider provider)
        {
            return TimeSpan.FromMilliseconds(GetElapsedThisPeriod(logItem, provider));
        }
    }
}
