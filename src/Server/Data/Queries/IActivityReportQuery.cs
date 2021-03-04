using NoCrast.Server.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoCrast.Server.Data.Queries
{
    public interface IActivityReportQuery
    {
        IActivityReportQuery Offset(int timeoffset);
        IActivityReportQuery Task(string id);
        IActivityReportQuery Start(DateTime time);
        IActivityReportQuery End(DateTime time);

        long PeriodSum(long startTime, long endTime);
    }
}
