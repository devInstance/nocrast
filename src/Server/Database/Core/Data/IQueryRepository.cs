using NoCrast.Server.Data.Queries;
using NoCrast.Server.Model;

namespace NoCrast.Server.Data
{
    public interface IQueryRepository
    {
        IActivityReportQuery GetActivityReportQuery(UserProfile currentProfile);
        IAggregateReportQuery GetAggregateReportQuery(UserProfile currentProfile);

        ITasksQuery GetTasksQuery(UserProfile currentProfile);
    }
}
