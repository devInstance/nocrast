using System;
using System.Collections.Generic;
using System.Text;

namespace NoCrast.Shared.Utils
{
    public static class DateTimeExtensions
    {
        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
            return dt.AddDays(-1 * diff).Date;
        }

        public static DateTime StartOfMonth(this DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, 1);
        }

        public static DateTime StartOfYear(this DateTime dt)
        {
            return new DateTime(dt.Year, 1, 1);
        }

        public static float ToHours(this long dt)
        {
            return (float) dt / (60 * 60 * 1000);
        }

        public static DateTime ToUTC(this DateTime dt, int timeoffset)
        {
            return dt.AddMinutes(timeoffset * -1);
        }
        public static DateTime ToLocal(this DateTime dt, int timeoffset)
        {
            return dt.AddMinutes(timeoffset);
        }
    }
}
