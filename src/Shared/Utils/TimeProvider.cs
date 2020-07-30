using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoCrast.Shared.Utils
{
    public class TimeProvider : ITimeProvider
    {
        public DateTime CurrentTime
        {
            get
            {
                return DateTime.UtcNow;
                //TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById("America/New_York");
                //return TimeZoneInfo.ConvertTime(DateTime.Now, easternZone);
            }
        }

        public int UtcTimeOffset => (int)TimeZoneInfo.Local.GetUtcOffset(DateTime.Now).TotalMinutes;
    }
}
