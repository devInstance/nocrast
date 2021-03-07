using DevInstance.LogScope;
using NoCrast.Server.Database;
using NoCrast.Server.Model;
using NoCrast.Shared.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoCrast.Server.Data.Queries.Postgres
{
    public class PostgresBaseQuery
    {
        public ITimeProvider TimeProvider { get; }

        protected IScopeLog log;

        protected ApplicationDbContext DB { get; }

        protected UserProfile CurrentProfile { get; }

        public PostgresBaseQuery(IScopeManager logManager,
                                    ITimeProvider timeProvider,
                                    ApplicationDbContext dB,
                                    UserProfile currentProfile)
        {
            log = logManager.CreateLogger(this);

            TimeProvider = timeProvider;
            DB = dB;

            CurrentProfile = currentProfile;

        }

    }
}
