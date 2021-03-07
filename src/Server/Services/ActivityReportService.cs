using DevInstance.LogScope;
using NoCrast.Server.Data;
using NoCrast.Server.Model;
using NoCrast.Shared.Model;
using NoCrast.Shared.Utils;
using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("NoCrast.ServerTests")]
namespace NoCrast.Server.Services
{

    public class ActivityReportService : BaseService
    {
        public ActivityReportService(IScopeManager logManager, ITimeProvider timeProvider, IQueryRepository query)
            : base(logManager, timeProvider, query)
        {
        }

        public ReportItem GetReport(UserProfile currentProfile, int timeoffset)
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

                var query = Repository.GetActivityReportQuery(currentProfile);
                query.Offset(timeoffset);

                for (int i = 0; i < 1; i++)
                {
                    var data = new long[columnsCount];
                    long maxValue = 0;
                    for (int n = 0; n < columnsCount; n++)
                    {
                        long startTime = dateRanges[n].Hour * 60 + dateRanges[n].Minute;
                        long endTime = dateRanges[n + 1].Hour * 60 + dateRanges[n + 1].Minute;

                        if (startTime > endTime)
                        {
                            endTime = startTime + interval;
                        }

                        var d = query.PeriodSum(startTime, endTime);

                        l.T($"{startTime}-{endTime} startTime:{dateRanges[n].TimeOfDay}, endTime:{dateRanges[n + 1].TimeOfDay} -> {d}");
                        if (d > maxValue)
                        {
                            maxValue = d;
                        }
                        data[n] = d;
                    }

                    var finalData = new ReportItem.Cell[columnsCount];
                    for (int n = 0; n < columnsCount; n++)
                    {
                        finalData[n] = new ReportItem.Cell
                        {
                            Details = String.Format("{0:F2} hours", (float)data[n] / 60.0), //TODO: i18n/l10n?
                            Value = (float)(data[n]) / (float)maxValue
                        };
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
