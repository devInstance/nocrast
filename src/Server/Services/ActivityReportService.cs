using DevInstance.LogScope;
using NoCrast.Server.Model;
using NoCrast.Server.Queries;
using NoCrast.Shared.Model;
using NoCrast.Shared.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("NoCrast.ServerTests")]
namespace NoCrast.Server.Services
{

    public class ActivityReportService
    {
        private IScopeLog log;

        public ITimeProvider TimeProvider { get; }
        public IActivityReportSelect Query { get; }

        public ActivityReportService(IScopeManager logManager, ITimeProvider timeProvider, IActivityReportSelect query)
        {
            log = logManager.CreateLogger(this);

            TimeProvider = timeProvider;
            Query = query;
        }

        public ReportItem GetActivityReport(UserProfile currentProfile, int timeoffset)
        {
            DateTime now = TimeProvider.CurrentTime;
            //DateTime startOfDay = TimeConverter.GetStartOfTheDayForTimeOffset(now, timeoffset);
            DateTime startOfDay = now.Date;

            var interval = 15;
            var columnsCount = 24 * 60 / interval;

            return GetActivityReport(currentProfile, timeoffset, interval, startOfDay, columnsCount);
        }

        internal ReportItem GetActivityReport(UserProfile currentProfile, int timeoffset, int interval, DateTime startOfDay, int columnsCount)
        //        public ActionResult<ReportItem> GetActivityReport(ActivityReportType? type, int? interval, string taskid, int timeoffset)

        {
            using (var l = log.TraceScope())
            {
                var result = new ReportItem()
                {
                    RiType = ReportItem.RIType.Unknown
                };

                var columnDate = startOfDay;
                result.Columns = new DateTime[columnsCount];
                var dateRanges = new DateTime[columnsCount + 1];
                for (int i = 0; i < columnsCount; i++)
                {
                    dateRanges[i] = columnDate;
                    result.Columns[i] = columnDate;
                    columnDate = columnDate.AddMinutes(interval);
                }
                dateRanges[columnsCount] = columnDate;

                result.Rows = new ReportItem.Row[1/*tasks.Count*/];

                for (int i = 0; i < 1; i++)
                {
                    var data = new long[columnsCount];
                    long maxValue = 0;
                    for (int n = 0; n < columnsCount; n++)
                    {
                        long startTime = dateRanges[n].Hour * 60 + dateRanges[n].Minute;
                        long endTime = dateRanges[n + 1].Hour * 60 + dateRanges[n + 1].Minute;

                        if(startTime > endTime)
                        {
                            endTime = startTime + interval;
                        }
                        var d = Query.GetTotalForPeriod(currentProfile, timeoffset, startTime, endTime);

                        l.T($"{startTime}-{endTime} startTime:{dateRanges[n].TimeOfDay}, endTime:{dateRanges[n + 1].TimeOfDay} -> {d}");
                        if (d > maxValue)
                        {
                            maxValue = d;
                        }
                        data[n] = d;
                    }

                    var finalData = new float[columnsCount];
                    for (int n = 0; n < columnsCount; n++)
                    {
                        finalData[n] = (float)(data[n]) / (float)maxValue;
                    }

                    result.Rows[i] = new ReportItem.Row
                    {
                        //TaskTitle = tasks[i].Title,
                        Data = finalData
                    };
                }

                return result;
            }
        }
    }
}
