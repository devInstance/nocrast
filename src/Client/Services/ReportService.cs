using NoCrast.Client.Services.Api;
using NoCrast.Shared.Logging;
using NoCrast.Shared.Model;
using NoCrast.Shared.Utils;
using System;
using System.Threading.Tasks;

namespace NoCrast.Client.Services
{
    public class ReportService : BaseService
    {
        public ITimeProvider TimeProvider { get; }
        protected IReportApi Api { get; }

        public ReportService(NotificationService notificationServ,
                            ITimeProvider provider,
                            ILogProvider logProvider,
                            IReportApi api) : base(notificationServ)
        {
            Api = api;
            TimeProvider = provider;
            Log = logProvider.CreateLogger(this);
            Log.D("constructor");
        }

        public async Task<ReportItem> GetReportAsync(ReportItem.RIType rIType, DateTime start)
        {
            using (var l = Log.DebugExScope())
            {
                ResetNetworkError();

                try
                {
                    switch (rIType)
                    {
                        case ReportItem.RIType.Weekly:
                            return await Api.GetWeeklyReportAsync(TimeProvider.UtcTimeOffset, start);
                        case ReportItem.RIType.Monthly:
                            return await Api.GetMonthlyReportAsync(TimeProvider.UtcTimeOffset, start);
                        case ReportItem.RIType.Daily:
                        default:
                            return await Api.GetDailyReportAsync(TimeProvider.UtcTimeOffset, start);
                    }
                }
                catch (Exception ex)
                {
                    NotifyNetworkError(ex);
                }
                return null;
            }
        }
    }
}
