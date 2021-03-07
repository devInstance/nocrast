using DevInstance.LogScope;
using NoCrast.Server.Database;
using NoCrast.Server.Model;
using NoCrast.Shared.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NoCrast.Server.Data.Queries.Postgres
{
    internal class PostgresTasksQuery : PostgresBaseQuery, ITasksQuery
    {
        private IQueryable<TimerTask> currentQuery;
        public PostgresTasksQuery(IScopeManager logManager, 
                                            ITimeProvider timeProvider, 
                                            ApplicationDbContext dB,
                                            UserProfile currentProfile)
            : base(logManager, timeProvider, dB, currentProfile)
        {
            currentQuery = from ts in DB.Tasks
                           where ts.Profile == CurrentProfile
                           select ts;
        }

        public List<TimerTask> SelectList()
        {
            return (from ts in currentQuery select ts).ToList();
        }
    }
}
