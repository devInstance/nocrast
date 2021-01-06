using NoCrast.Shared.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoCrast.Client.Services.Api
{
    public interface IReportApi
    {
        Task<ReportItem> GetWeeklyReportAsync(int timeoffset, DateTime startTime);
        Task<ReportItem> GetMonthlyReportAsync(int timeoffset, DateTime startTime);
        Task<ReportItem> GetDailyReportAsync(int timeoffset, DateTime startTime);

        Task<ReportItem> GetActivityReportAsync(int timeoffset);
    }
}
