using NoCrast.Client.Services.Api;
using NoCrast.Shared.Model;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace NoCrast.Client.Services.Net
{
    public class ReportApi : ApiBase, IReportApi
    {
        private const string Controller = "api/data/reports";

        public ReportApi(HttpClient http) : base(http)
        {
        }

        public async Task<ReportItem> GetDailyReportAsync(int timeoffset, DateTime startTime)
        {
            return await httpClient.GetFromJsonAsync<ReportItem>($"{Controller}/aggregate/daily?timeoffset={timeoffset}&start={startTime}");
        }

        public async Task<ReportItem> GetMonthlyReportAsync(int timeoffset, DateTime startTime)
        {
            return await httpClient.GetFromJsonAsync<ReportItem>($"{Controller}/aggregate/monthly?timeoffset={timeoffset}&start={startTime}");
        }

        public async Task<ReportItem> GetWeeklyReportAsync(int timeoffset, DateTime startTime)
        {
            return await httpClient.GetFromJsonAsync<ReportItem>($"{Controller}/aggregate/weekly?timeoffset={timeoffset}&start={startTime}");
        }

        public async Task<ReportItem> GetActivityReportAsync(int timeoffset, ReportItem.RIType type, ReportItem.RIMode mode, DateTime startTime, DateTime? endTime)
        {
            var url = $"{Controller}/activity?timeoffset={timeoffset}&type={type}&mode={mode}&start={startTime}";
            if(endTime.HasValue)
            {
                url += $"&end={endTime.Value}";
            }
            return await httpClient.GetFromJsonAsync<ReportItem>(url);
        }
    }
}
