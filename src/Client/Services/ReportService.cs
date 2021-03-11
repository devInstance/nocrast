using DevInstance.LogScope;
using NoCrast.Client.Services.Api;
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
                            IScopeManager logProvider,
                            IReportApi api) : base(notificationServ)
        {
            Api = api;
            TimeProvider = provider;
            Log = logProvider.CreateLogger(this);
            Log.D("constructor");
        }

        public async Task<ReportItem> GetAggregateReportAsync(ReportItem.RIType rIType, DateTime start)
        {
            using (var l = Log.TraceScope())
            {
                ResetNetworkError();


                try
                {
                    return await Api.GetAggregateReportAsync(TimeProvider.UtcTimeOffset, rIType, start);
                }
                catch (Exception ex)
                {
                    NotifyNetworkError(ex);
                }
                return null;
            }
        }

        public async Task<ReportItem> GetActivityReportAsync(ReportItem.RIType rIType, ReportItem.RIMode mode, DateTime start)
        {
            using (var l = Log.TraceScope())
            {
                ResetNetworkError();

                try
                {
                    return await Api.GetActivityReportAsync(TimeProvider.UtcTimeOffset, rIType, mode, start, null);
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
