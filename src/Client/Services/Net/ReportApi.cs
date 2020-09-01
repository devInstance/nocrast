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
        private const string Controller = "api/data/report";

        public ReportApi(HttpClient http) : base(http)
        {
        }

        public async Task<ReportItem> GetDailyReportAsync(int timeoffset, DateTime startTime)
        {
            return await httpClient.GetFromJsonAsync<ReportItem>($"{Controller}/daily?timeoffset={timeoffset}&start={startTime}");
        }

        public async Task<ReportItem> GetMonthlyReportAsync(int timeoffset, DateTime startTime)
        {
            return await httpClient.GetFromJsonAsync<ReportItem>($"{Controller}/monthly?timeoffset={timeoffset}&start={startTime}");
        }

        public async Task<ReportItem> GetWeeklyReportAsync(int timeoffset, DateTime startTime)
        {
            return await httpClient.GetFromJsonAsync<ReportItem>($"{Controller}/weekly?timeoffset={timeoffset}&start={startTime}");
        }
    }
}
