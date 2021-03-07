using NoCrast.Server.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoCrast.Server.Data.Queries
{
    public interface IAggregateReportQuery
    {
        long PeriodSum(DateTime start, DateTime end);
        IAggregateReportQuery Task(TimerTask timerTask);
    }
}
