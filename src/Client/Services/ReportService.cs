using Blazored.LocalStorage;
using NoCrast.Client.ModelExtensions;
using NoCrast.Client.ModelViews;
using NoCrast.Client.Services.Api;
using NoCrast.Shared.Logging;
using NoCrast.Shared.Model;
using NoCrast.Shared.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace NoCrast.Client.Services
{
    public class ReportService : BaseService
    {
        public ITimeProvider TimeProvider { get; }
        protected IReportApi Api { get; }

        public ReportService(ITimeProvider provider,
                            ILogProvider logProvider,
                            IReportApi api)
        {
            Api = api;
            TimeProvider = provider;
            Log = logProvider.CreateLogger(this);
            Log.D("constructor");
        }

        public async Task<ReportItem> GetReportAsync(ReportItem.RIType rIType)
        {
            using (var l = Log.DebugScope())
            {
                ResetNetworkError();

                try
                {
                    switch (rIType)
                    {
                        case ReportItem.RIType.Weekly:
                            return await Api.GetWeeklyReportAsync(TimeProvider.UtcTimeOffset, TimeProvider.CurrentTime);
                        case ReportItem.RIType.Monthly:
                            return await Api.GetMonthlyReportAsync(TimeProvider.UtcTimeOffset, TimeProvider.CurrentTime);
                        case ReportItem.RIType.Daily:
                        default:
                            return await Api.GetDailyReportAsync(TimeProvider.UtcTimeOffset, TimeProvider.CurrentTime);
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
