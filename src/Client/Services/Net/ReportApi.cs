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

        public async Task<ReportItem> GetAggregateReportAsync(int timeoffset, ReportItem.RIType type, DateTime startTime)
        {
            return await httpClient.GetFromJsonAsync<ReportItem>($"{Controller}/aggregate?timeoffset={timeoffset}&type={type}&start={startTime}");
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
