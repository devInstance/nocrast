﻿using System;

namespace NoCrast.Shared.Model
{
    public enum TimeLogResultType
    {
        Day,
        Week,
        All,
        WeekExcludeToday,
        AllExcludeThisWeek
    }
    public class TimeLogItem : ModelItem
    {
        public DateTime StartTime { get; set; }

        public long ElapsedMilliseconds { get; set; }

        public string Notes { get; set; }
    }
}
