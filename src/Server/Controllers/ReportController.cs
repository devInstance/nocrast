using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoCrast.Server.Database;
using NoCrast.Server.Model;
using NoCrast.Server.Utils;
using NoCrast.Shared.Model;
using NoCrast.Shared.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NoCrast.Server.Controllers
{
    [ApiController]
    [Route("api/data/report")]
    public class ReportController : UserBaseController
    {
        public ITimeProvider TimeProvider { get; }

        public ReportController(ApplicationDbContext d,
                                UserManager<ApplicationUser> userManager,
                                ITimeProvider timeProvider)
            : base(d, userManager)
        {
            TimeProvider = timeProvider;
        }

        [Authorize]
        [HttpGet]
        [Route("daily")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<ReportItem> GetDailyReport(int timeoffset, DateTime start)
        {
            return HandleWebRequest<ReportItem>(() =>
            {
                int DaysCount = 7;
                // initialize result
                var result = new ReportItem()
                {
                    RiType = ReportItem.RIType.Daily
                };

                DateTime now = TimeProvider.CurrentTime;
                DateTime startOfTheWeek = TimeConverter.GetStartOfTheWeekForTimeOffset(now, timeoffset);

                var columnDate = startOfTheWeek;
                result.Columns = new DateTime[DaysCount];
                var dateRanges = new DateTime[DaysCount + 1];
                for (int i = 0; i < DaysCount; i ++)
                {
                    dateRanges[i] = columnDate;
                    result.Columns[i] = TimeConverter.ConvertToLocal(columnDate, timeoffset);
                    columnDate = columnDate.AddDays(1);
                }
                dateRanges[DaysCount] = columnDate;

                var tasks = (from tks in DB.Tasks where tks.Profile == CurrentProfile select tks).ToList();

                result.Rows = new ReportItem.Row[tasks.Count];
                for(int i =0; i < tasks.Count; i ++)
                {
                    var data = new float[DaysCount + 1];
                    float total = 0F;
                    for(int n = 0; n < DaysCount; n++)
                    {
                        var d = (from tl in DB.TimeLog
                                  where tl.TaskId == tasks[i].Id
                                  && tl.StartTime >= dateRanges[n] && tl.StartTime <= dateRanges[n + 1]
                                  select tl.ElapsedMilliseconds).Sum();

                        data[n] = d;
                        total += d;
                    }
                    data[DaysCount] = total;

                    result.Rows[i] = new ReportItem.Row
                    {
                        TaskTitle = tasks[i].Title,
                        Data = data
                    };
                }

                return Ok(result);
            });
        }

        [Authorize]
        [HttpGet]
        [Route("weekly")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<ReportItem> GetWeeklyReport(int timeoffset, DateTime start)
        {
            return HandleWebRequest<ReportItem>(() =>
            {
                int DaysCount = 5;
                // initialize result
                var result = new ReportItem()
                {
                    RiType = ReportItem.RIType.Weekly
                };

                DateTime now = TimeProvider.CurrentTime;
                DateTime startOfTheWeek = TimeConverter.GetStartOfTheWeekForTimeOffset(now, timeoffset).AddDays(-4 * 7);

                var columnDate = startOfTheWeek;
                result.Columns = new DateTime[DaysCount];
                var dateRanges = new DateTime[DaysCount + 1];
                for (int i = 0; i < DaysCount; i++)
                {
                    dateRanges[i] = columnDate;
                    result.Columns[i] = TimeConverter.ConvertToLocal(columnDate, timeoffset);
                    columnDate = columnDate.AddDays(7);
                }
                dateRanges[DaysCount] = columnDate;

                var tasks = (from tks in DB.Tasks where tks.Profile == CurrentProfile select tks).ToList();

                result.Rows = new ReportItem.Row[tasks.Count];
                for (int i = 0; i < tasks.Count; i++)
                {
                    var data = new float[DaysCount + 1];
                    float total = 0F;
                    for (int n = 0; n < DaysCount; n++)
                    {
                        var d = (from tl in DB.TimeLog
                                 where tl.TaskId == tasks[i].Id
                                 && tl.StartTime >= dateRanges[n] && tl.StartTime <= dateRanges[n + 1]
                                 select tl.ElapsedMilliseconds).Sum();

                        data[n] = d;
                        total += d;
                    }
                    data[DaysCount] = total;

                    result.Rows[i] = new ReportItem.Row
                    {
                        TaskTitle = tasks[i].Title,
                        Data = data
                    };
                }

                return Ok(result);
            });
        }

        [Authorize]
        [HttpGet]
        [Route("monthly")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<ReportItem> GetMonthlyReport(int timeoffset, DateTime start)
        {
            return HandleWebRequest<ReportItem>(() =>
            {
                int DaysCount = 12;
                // initialize result
                var result = new ReportItem()
                {
                    RiType = ReportItem.RIType.Monthly
                };

                DateTime now = TimeProvider.CurrentTime;
                DateTime startOfTheYear = TimeConverter.GetStartOfTheYearForTimeOffset(now, timeoffset);

                var columnDate = startOfTheYear;
                result.Columns = new DateTime[DaysCount];
                var dateRanges = new DateTime[DaysCount + 1];
                for (int i = 0; i < DaysCount; i++)
                {
                    dateRanges[i] = columnDate;
                    result.Columns[i] = TimeConverter.ConvertToLocal(columnDate, timeoffset);
                    columnDate = columnDate.AddMonths(1);
                }
                dateRanges[DaysCount] = columnDate;

                var tasks = (from tks in DB.Tasks where tks.Profile == CurrentProfile select tks).ToList();

                result.Rows = new ReportItem.Row[tasks.Count];
                for (int i = 0; i < tasks.Count; i++)
                {
                    var data = new float[DaysCount + 1];
                    float total = 0F;
                    for (int n = 0; n < DaysCount; n++)
                    {
                        var d = (from tl in DB.TimeLog
                                 where tl.TaskId == tasks[i].Id
                                 && tl.StartTime >= dateRanges[n] && tl.StartTime <= dateRanges[n + 1]
                                 select tl.ElapsedMilliseconds).Sum();

                        data[n] = d;
                        total += d;
                    }
                    data[DaysCount] = total;

                    result.Rows[i] = new ReportItem.Row
                    {
                        TaskTitle = tasks[i].Title,
                        Data = data
                    };
                }

                return Ok(result);
            });
        }
    }
}
