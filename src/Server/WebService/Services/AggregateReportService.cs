using DevInstance.LogScope;
using NoCrast.Server.Data;
using NoCrast.Server.Model;
using NoCrast.Shared.Model;
using NoCrast.Shared.Utils;
using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("NoCrast.Server.WebService.Tests")]
namespace NoCrast.Server.Services
{
    public class AggregateReportService : BaseService
    {
        public AggregateReportService(IScopeManager logManager, ITimeProvider timeProvider, IQueryRepository query)
            : base(logManager, timeProvider, query)
        {
        }

        private ReportItem GetReport(UserProfile currentProfile, ReportItem.RIType tp, DateTime start, int ColumnsCount)
        {
            using (var l = log.TraceScope())
            {
                l.T($"start: {start}");
                var result = new ReportItem()
                {
                    RiType = tp,
                    StartDate = start
                };

                var columnDate = start;
                result.Columns = new DateTime[ColumnsCount];
                var dateRanges = new DateTime[ColumnsCount + 1];
                for (int i = 0; i < ColumnsCount; i++)
                {
                    dateRanges[i] = columnDate;
                    result.Columns[i] = columnDate;
                    switch (tp)
                    {
                        case ReportItem.RIType.Daily:
                            columnDate = columnDate.AddDays(1);
                            break;
                        case ReportItem.RIType.Weekly:
                            columnDate = columnDate.AddDays(7);
                            break;
                        case ReportItem.RIType.Monthly:
                        default:
                            columnDate = columnDate.AddMonths(1);
                            break;
                    }
                }
                dateRanges[ColumnsCount] = columnDate;
                result.EndDate = columnDate.AddDays(-1);

                var tasks = Repository.GetTasksQuery(currentProfile).SelectList();

                result.Rows = new ReportItem.Row[tasks.Count];

                for (int i = 0; i < tasks.Count; i++)
                {
                    l.T($"Task {tasks[i].Id} {tasks[i].PublicId}");

                    var reportQuery = Repository.GetAggregateReportQuery(currentProfile).Task(tasks[i]);
                    var data = new ReportItem.Cell[ColumnsCount + 1];
                    float total = 0F;
                    for (int n = 0; n < ColumnsCount; n++)
                    {
                        var d = reportQuery.PeriodSum(dateRanges[n], dateRanges[n + 1]);
                        l.T($"report cell between: {dateRanges[n]} and {dateRanges[n+1]}, sum {d}");

                        data[n] = new ReportItem.Cell
                        {
                            Value = d
                        };
                        total += d;
                    }
                    data[ColumnsCount] = new ReportItem.Cell { Value = total };

                    result.Rows[i] = new ReportItem.Row
                    {
                        TaskTitle = tasks[i].Title,
                        Data = data
                    };
                }

                return result;
            }
        }

        internal ReportItem GetReport(UserProfile currentProfile, ReportItem.RIType type, int timeoffset, DateTime start)
        {
            using (var l = log.TraceScope())
            {
                DateTime startPeriod;
                int columnCount;
                switch (type)
                {
                    case ReportItem.RIType.Daily:
                        columnCount = 7;
                        startPeriod = TimeConverter.GetStartOfTheWeekForTimeOffset(start, timeoffset);
                        break;
                    case ReportItem.RIType.Weekly:
                        columnCount = 5;
                        startPeriod = TimeConverter.GetStartOfTheWeekForTimeOffset(start, timeoffset).AddDays(-4 * 7);
                        break;
                    case ReportItem.RIType.Monthly:
                        columnCount = 12;
                        startPeriod = TimeConverter.GetStartOfTheYearForTimeOffset(start, timeoffset);
                        break;
                    default:
                        throw new Exception("Report type is not supported");
                }

                return GetReport(currentProfile, type, startPeriod, columnCount);
            }
        }
    }
}
