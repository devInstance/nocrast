using DevInstance.LogScope;
using NoCrast.Server.Data.Queries;
using NoCrast.Server.Model;
using NoCrast.Shared.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("NoCrast.Server.PostgressTests")]
namespace NoCrast.Server.Database.Postgres.Data.Queries
{
    internal class PostgresAggregateReportQuery : PostgresBaseQuery, IAggregateReportQuery
    {
        private IQueryable<TimeLog> currentQuery;

        public PostgresAggregateReportQuery(IScopeManager logManager,
                                            ITimeProvider timeProvider,
                                            ApplicationDbContext dB,
                                            UserProfile currentProfile)
            : base(logManager, timeProvider, dB, currentProfile)
        {
            currentQuery = from tl in DB.TimeLog
                           join ts in DB.Tasks on tl.Task equals ts
                           where ts.Profile == CurrentProfile
                           select tl;
        }


        public long PeriodSum(DateTime start, DateTime end)
        {
            return (from tl in currentQuery
                    where tl.StartTime >= start && tl.StartTime < end
                    select tl.ElapsedMilliseconds).Sum();
        }

        public IAggregateReportQuery Task(TimerTask timerTask)
        {
            currentQuery = from tl in currentQuery
                           where tl.TaskId == timerTask.Id
                           select tl;
            return this;
        }
    }
}
