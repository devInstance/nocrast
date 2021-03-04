﻿using DevInstance.LogScope;
using NoCrast.Server.Database;
using NoCrast.Server.Model;
using NoCrast.Shared.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NoCrast.Server.Data.Queries.Postgres
{
    internal class PostgresActivityReportQuery : IActivityReportQuery
    {
        public ITimeProvider TimeProvider { get; }

        private IScopeLog log;

        private ApplicationDbContext DB { get; }

        private UserProfile CurrentProfile { get; }
        private int Timeoffset { get; set; }

        class QueryType {
            public TimeLog log;
            public TimerTask task;
        }
        private IQueryable<QueryType> CurrentQuery;

        public PostgresActivityReportQuery(IScopeManager logManager, 
                                            ITimeProvider timeProvider, 
                                            ApplicationDbContext dB,
                                            UserProfile currentProfile)
        {
            log = logManager.CreateLogger(this);

            TimeProvider = timeProvider;
            DB = dB;

            CurrentProfile = currentProfile;

            CurrentQuery = from tl in DB.TimeLog
                           join ts in DB.Tasks on tl.Task equals ts
                           where ts.Profile == CurrentProfile
                           select new QueryType { log = tl, task = ts };
        }

        public IActivityReportQuery Offset(int timeoffset)
        {
            Timeoffset = timeoffset;
            return this;
        }

        public IActivityReportQuery Task(string id)
        {
            CurrentQuery = (from q in CurrentQuery where q.task.PublicId == id select q);
            return this;
        }

        public IActivityReportQuery Start(DateTime time)
        {
            CurrentQuery = (from q in CurrentQuery where q.log.StartTime >= time select q);
            return this;
        }

        public IActivityReportQuery End(DateTime time)
        {
            CurrentQuery = (from q in CurrentQuery where q.log.StartTime.AddMilliseconds(q.log.ElapsedMilliseconds) <= time select q);
            return this;
        }

        public long PeriodSum(long startTime, long endTime)
        {
            // https://scicomp.stackexchange.com/questions/26258/the-easiest-way-to-find-intersection-of-two-intervals
            var priod = (from q in CurrentQuery
                     where !(startTime > (q.log.StartTime.AddMinutes(Timeoffset).Hour * 60 + q.log.StartTime.AddMinutes(Timeoffset).Minute + (q.log.ElapsedMilliseconds / 1000 / 60))
                     || (endTime < q.log.StartTime.AddMinutes(Timeoffset).Hour * 60 + q.log.StartTime.AddMinutes(Timeoffset).Minute))
                     select Math.Min(endTime, (q.log.StartTime.AddMinutes(Timeoffset).Hour * 60 + q.log.StartTime.AddMinutes(Timeoffset).Minute + (q.log.ElapsedMilliseconds / 1000 / 60)))
                            - Math.Max(startTime, q.log.StartTime.AddMinutes(Timeoffset).Hour * 60 + q.log.StartTime.AddMinutes(Timeoffset).Minute)
                            );

            //var test = priod.Count();

            //log.T($"records = {test}");

            return priod.Sum();
        }

    }
}