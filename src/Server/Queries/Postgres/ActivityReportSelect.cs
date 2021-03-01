using DevInstance.LogScope;
using NoCrast.Server.Database;
using NoCrast.Server.Model;
using NoCrast.Shared.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoCrast.Server.Queries.Postgress
{
    public class ActivityReportSelect : IActivityReportSelect
    {
        public ITimeProvider TimeProvider { get; }

        private IScopeLog log;

        protected ApplicationDbContext DB { get; }

        public ActivityReportSelect(IScopeManager logManager, ITimeProvider timeProvider, ApplicationDbContext dB)
        {
            log = logManager.CreateLogger(this);

            TimeProvider = timeProvider;
            DB = dB;
        }

        public long GetTotalForPeriod(UserProfile currentProfile, int timeoffset, long startTime, long endTime)
        {
            // https://scicomp.stackexchange.com/questions/26258/the-easiest-way-to-find-intersection-of-two-intervals
            var q = (from tl in DB.TimeLog
                     join ts in DB.Tasks on tl.Task equals ts
                     where ts.Profile == currentProfile &&
                     !(startTime > (tl.StartTime.AddMinutes(timeoffset).Hour * 60 + tl.StartTime.AddMinutes(timeoffset).Minute + (tl.ElapsedMilliseconds / 1000 / 60))
                     || (endTime < tl.StartTime.AddMinutes(timeoffset).Hour * 60 + tl.StartTime.AddMinutes(timeoffset).Minute))
                     select Math.Min(endTime, (tl.StartTime.AddMinutes(timeoffset).Hour * 60 + tl.StartTime.AddMinutes(timeoffset).Minute + (tl.ElapsedMilliseconds / 1000 / 60)))
                            - Math.Max(startTime, tl.StartTime.AddMinutes(timeoffset).Hour * 60 + tl.StartTime.AddMinutes(timeoffset).Minute)
                            );

            var test = q.Count();

            log.T($"records = {test}");

            return q.Sum();
        }
    }
}
