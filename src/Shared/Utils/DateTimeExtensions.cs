﻿using System;
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

        public static float ToHours(this long dt)
        {
            return (float) dt / (60 * 60 * 1000);
        }
    }
}
