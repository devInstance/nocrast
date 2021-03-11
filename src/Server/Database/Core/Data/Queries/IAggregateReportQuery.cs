using System;

namespace NoCrast.Server.Data.Queries
{
    public interface IAggregateReportQuery
    {
        long PeriodSum(DateTime start, DateTime end);
        IAggregateReportQuery Task(string id);
    }
}
