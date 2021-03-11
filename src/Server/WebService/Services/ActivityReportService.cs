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

    public class ActivityReportService : BaseService
    {
        public ActivityReportService(IScopeManager logManager, ITimeProvider timeProvider, IQueryRepository query)
            : base(logManager, timeProvider, query)
        {
        }

        public ReportItem GetReport(UserProfile currentProfile, int timeoffset, ReportItem.RIType type, ReportItem.RIMode mode, int? interval, DateTime? start)
        {
            DateTime now = TimeProvider.CurrentTime;
            DateTime startOfDay = TimeConverter.GetStartOfTheDayForTimeOffset(now, timeoffset);
            //DateTime startOfDay = now.Date;
            if(start.HasValue)
            {
                startOfDay = TimeConverter.ConvertToUtc(start.Value.Date, timeoffset);
            }

            var ninterval = 15;
            if(interval.HasValue)
            {
                ninterval = interval.Value;
            }
            var columnsCount = 24 * 60 / ninterval;

            return GetActivityReport(currentProfile, type, mode, timeoffset, ninterval, startOfDay, columnsCount);
        }

        internal ReportItem GetActivityReport(UserProfile currentProfile, 
                                                ReportItem.RIType type, 
                                                ReportItem.RIMode mode, 
                                                int timeoffset, 
                                                int interval, 
                                                DateTime startOfDay, 
                                                int columnsCount)
        {
            using (var l = log.TraceScope())
            {
                var result = new ReportItem()
                {
                    RiType = type,
                    RiMode = mode,
                    StartDate = startOfDay
                };

                TimerTask[] tasks;
                if(mode == ReportItem.RIMode.ByTask)
                {
                    tasks = Repository.GetTasksQuery(currentProfile).SelectList().ToArray();
                }
                else
                {
                    tasks = new[] { new TimerTask { Title = "All" } };
                }

                var columnDate = startOfDay;
                result.Columns = new DateTime[columnsCount];
                var dateRanges = new DateTime[columnsCount + 1];
                for (int i = 0; i < columnsCount; i++)
                {
                    dateRanges[i] = columnDate;
                    result.Columns[i] = TimeConverter.ConvertToLocal(columnDate, timeoffset);
                    columnDate = columnDate.AddMinutes(interval);
                }
                dateRanges[columnsCount] = columnDate;

                result.Rows = new ReportItem.Row[tasks.Length];
                
                
                //query.Start(startOfDay);
                //query.End(columnDate);

                for (int i = 0; i < tasks.Length; i++)
                {
                    var query = Repository.GetActivityReportQuery(currentProfile);

                    switch (type)
                    {
                        case ReportItem.RIType.Weekly:
                            {
                                query.Start(startOfDay);

                                var end = startOfDay.AddDays(7);
                                query.End(end);
                                result.EndDate = end;
                            }
                            break;
                        case ReportItem.RIType.Monthly:
                            {
                                query.Start(startOfDay);

                                var end = startOfDay.AddMonths(1);
                                query.End(end);
                                result.EndDate = end;
                            }
                            break;
                        case ReportItem.RIType.Yearly:
                            {
                                query.Start(startOfDay);

                                var end = startOfDay.AddYears(1);
                                query.End(end);
                                result.EndDate = end;
                            }
                            break;
                        default:
                            result.EndDate = startOfDay;
                            break;
                    }

                    if (tasks[i].PublicId != null)
                    {
                        query.Task(tasks[i].PublicId);
                    }
                    l.T($"***** Task {tasks[i].Title} startTime:{result.StartDate}, endTime:{result.EndDate} -> {type}");

                    var data = new long[columnsCount];
                    long maxValue = 1;
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

                        if (d > 0 && d > maxValue)
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
                        Title = tasks[i].Title,
                        Data = finalData
                    };
                }

                return result;
            }
        }
    }
}
