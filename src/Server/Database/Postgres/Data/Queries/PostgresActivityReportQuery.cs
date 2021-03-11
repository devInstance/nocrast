using DevInstance.LogScope;
using NoCrast.Server.Data.Queries;
using NoCrast.Server.Model;
using NoCrast.Shared.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NoCrast.Server.Database.Postgres.Data.Queries
{
    internal class PostgresActivityReportQuery : PostgresBaseQuery, IActivityReportQuery
    {
        class QueryType {
            public TimeLog log;
            public TimerTask task;
        }
        private IQueryable<QueryType> currentQuery;

        public PostgresActivityReportQuery(IScopeManager logManager, 
                                            ITimeProvider timeProvider, 
                                            ApplicationDbContext dB,
                                            UserProfile currentProfile)
            : base(logManager, timeProvider, dB, currentProfile)
        {
            currentQuery = from tl in DB.TimeLog
                           join ts in DB.Tasks on tl.Task equals ts
                           where ts.Profile == CurrentProfile
                           select new QueryType { log = tl, task = ts };
        }

        public IActivityReportQuery Task(string id)
        {
            currentQuery = (from q in currentQuery where q.task.PublicId == id select q);
            return this;
        }

        public IActivityReportQuery Start(DateTime time)
        {
            currentQuery = (from q in currentQuery where q.log.StartTime >= time select q);
            return this;
        }

        public IActivityReportQuery End(DateTime time)
        {
            currentQuery = (from q in currentQuery where q.log.StartTime <= time select q);
            return this;
        }

        public long PeriodSum(long startTime, long endTime)
        {
            // https://scicomp.stackexchange.com/questions/26258/the-easiest-way-to-find-intersection-of-two-intervals
            var priod = (from q in currentQuery
                     where !(startTime > (q.log.StartTime.Hour * 60 + q.log.StartTime.Minute + (q.log.ElapsedMilliseconds / 1000 / 60))
                     || (endTime < q.log.StartTime.Hour * 60 + q.log.StartTime.Minute))
                     select Math.Min(endTime, (q.log.StartTime.Hour * 60 + q.log.StartTime.Minute + (q.log.ElapsedMilliseconds / 1000 / 60)))
                            - Math.Max(startTime, q.log.StartTime.Hour * 60 + q.log.StartTime.Minute)
                            );

            //var test = priod.Count();

            //log.T($"records = {test}");

            return priod.Sum();
        }

    }
}
