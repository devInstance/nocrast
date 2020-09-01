using NoCrast.Shared.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoCrast.Server.Utils
{
    public class TimeConverter
    {
        public static DateTime GetStartOfTheDayForTimeOffset(DateTime utcNow, int timeoffset)
        {
            DateTime locaNow = ConvertToLocal(utcNow, timeoffset);

            return ConvertToUtc(locaNow.Date, timeoffset);
        }

        public static DateTime GetStartOfTheWeekForTimeOffset(DateTime utcNow, int timeoffset)
        {
            DateTime locaNow = ConvertToLocal(utcNow, timeoffset);

            return ConvertToUtc(locaNow.StartOfWeek(DayOfWeek.Monday), timeoffset);
        }

        public static DateTime GetStartOfTheMonthForTimeOffset(DateTime utcNow, int timeoffset)
        {
            DateTime locaNow = ConvertToLocal(utcNow, timeoffset);

            return ConvertToUtc(new DateTime(locaNow.Year, locaNow.Month, 1), timeoffset);
        }

        public static DateTime GetStartOfTheYearForTimeOffset(DateTime utcNow, int timeoffset)
        {
            DateTime locaNow = ConvertToLocal(utcNow, timeoffset);

            return ConvertToUtc(new DateTime(locaNow.Year, 1, 1), timeoffset);
        }

        public static DateTime ConvertToLocal(DateTime utcNow, int timeoffset)
        {
            return utcNow.AddMinutes(timeoffset);
        }

        public static DateTime ConvertToUtc(DateTime locaNow, int timeoffset)
        {
            return locaNow.AddMinutes(timeoffset * -1);
        }
    }
}
