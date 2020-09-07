using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoCrast.Server.Database;
using NoCrast.Server.Model;
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

        private ReportItem GetReport(ReportItem.RIType tp, DateTime start, int ColumnsCount)
        {
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
                switch(tp)
                {
                    case ReportItem.RIType.Daily:
                        columnDate = columnDate.AddDays(1);
                        break;
                    case ReportItem.RIType.Weekly:
                        columnDate = columnDate.AddDays(7);
                        break;
                    case ReportItem.RIType.Monthly:
                        columnDate = columnDate.AddMonths(1);
                        break;
                }
            }
            dateRanges[ColumnsCount] = columnDate;
            result.EndDate = columnDate.AddDays(-1);

            var tasks = (from tks in DB.Tasks where tks.Profile == CurrentProfile orderby tks.CreateDate select tks).ToList();

            result.Rows = new ReportItem.Row[tasks.Count];
            for (int i = 0; i < tasks.Count; i++)
            {
                var data = new float[ColumnsCount + 1];
                float total = 0F;
                for (int n = 0; n < ColumnsCount; n++)
                {
                    var d = (from tl in DB.TimeLog
                             where tl.TaskId == tasks[i].Id
                             && tl.StartTime >= dateRanges[n] && tl.StartTime < dateRanges[n + 1]
                             select tl.ElapsedMilliseconds).Sum();

                    data[n] = d;
                    total += d;
                }
                data[ColumnsCount] = total;

                result.Rows[i] = new ReportItem.Row
                {
                    TaskTitle = tasks[i].Title,
                    Data = data
                };
            }

            return result;
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
                DateTime startOfTheWeek = TimeConverter.GetStartOfTheWeekForTimeOffset(start, timeoffset);

                var result = GetReport(ReportItem.RIType.Daily, startOfTheWeek, 7);

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

                DateTime startOfTheWeek = TimeConverter.GetStartOfTheWeekForTimeOffset(start, timeoffset).AddDays(-4 * 7);

                var result = GetReport(ReportItem.RIType.Weekly, startOfTheWeek, 5);

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
                DateTime startOfTheYear = TimeConverter.GetStartOfTheYearForTimeOffset(start, timeoffset);

                var result = GetReport(ReportItem.RIType.Monthly, startOfTheYear, 12);

                return Ok(result);
            });
        }
    }
}
