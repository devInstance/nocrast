using NoCrast.Server.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoCrast.Server.Queries
{
    public interface IActivityReportSelect
    {
        long GetTotalForPeriod(UserProfile currentProfile, int timeoffset, long startTime, long endTime);
    }
}
