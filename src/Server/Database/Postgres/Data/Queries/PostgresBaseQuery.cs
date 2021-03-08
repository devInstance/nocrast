using DevInstance.LogScope;
using NoCrast.Server.Model;
using NoCrast.Shared.Utils;

namespace NoCrast.Server.Database.Postgres.Data.Queries
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
