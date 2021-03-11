using DevInstance.LogScope;
using Microsoft.AspNetCore.Identity;
using NoCrast.Server.Data;
using NoCrast.Server.Data.Queries;
using NoCrast.Server.Model;
using NoCrast.Shared.Utils;

namespace NoCrast.Server.Database.Postgres.Data.Queries
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

        public IActivityReportQuery GetActivityReportQuery(UserProfile userProfile)
        {
            return new PostgresActivityReportQuery(LogManager, TimeProvider, DB, userProfile);
        }

        public IAggregateReportQuery GetAggregateReportQuery(UserProfile userProfile)
        {
            return new PostgresAggregateReportQuery(LogManager, TimeProvider, DB, userProfile);
        }

        public ITasksQuery GetTasksQuery(UserProfile userProfile)
        {
            return new PostgresTasksQuery(LogManager, TimeProvider, DB, userProfile);
        }
    }
}
