using NoCrast.Shared.Model;
using System;
using System.Threading.Tasks;

namespace NoCrast.Client.Services.Api
{
    public interface IReportApi
    {
        Task<ReportItem> GetAggregateReportAsync(int timeoffset, ReportItem.RIType type, DateTime startTime);

        Task<ReportItem> GetActivityReportAsync(int timeoffset, ReportItem.RIType type, ReportItem.RIMode mode, DateTime startTime, DateTime? endTime);
    }
}
