using DevInstance.LogScope;
using Microsoft.AspNetCore.Identity;
using NoCrast.Server.Database;
using NoCrast.Server.Model;
using NoCrast.Shared.Utils;
using System;
using System.Security.Claims;

namespace NoCrast.Server.Data.Queries.Postgres
{
    public class PostgresQueryRepository : IQueryRepository
    {
        protected ApplicationDbContext DB { get; }
        public ITimeProvider TimeProvider { get; }

        private IScopeLog log;
        private IScopeManager LogManager;

        private UserManager<ApplicationUser> UserManager { get; }

        public PostgresQueryRepository(IScopeManager logManager, ITimeProvider timeProvider, ApplicationDbContext dB, UserManager<ApplicationUser> userManager)
        {
            LogManager = logManager;
            log = logManager.CreateLogger(this);

            TimeProvider = timeProvider;
            DB = dB;
        }

        private PostgresActivityReportQuery postgresActivityReportQuery;
        public IActivityReportQuery GetActivityReportQuery(UserProfile userProfile)
        {
            if (postgresActivityReportQuery == null)
            {
                postgresActivityReportQuery = new PostgresActivityReportQuery(LogManager, TimeProvider, DB, userProfile);
            }
            return postgresActivityReportQuery;
        }

        private PostgresAggregateReportQuery postgresAggregateReportQuery;
        public IAggregateReportQuery GetAggregateReportQuery(UserProfile userProfile)
        {
            if (postgresAggregateReportQuery == null)
            {
                postgresAggregateReportQuery = new PostgresAggregateReportQuery(LogManager, TimeProvider, DB, userProfile);
            }
            return postgresAggregateReportQuery;
        }

        private PostgresTasksQuery postgresTasksQuery;
        public ITasksQuery GetTasksQuery(UserProfile userProfile)
        {
            if (postgresTasksQuery == null)
            {
                postgresTasksQuery = new PostgresTasksQuery(LogManager, TimeProvider, DB, userProfile);
            }
            return postgresTasksQuery;
        }
    }
}
