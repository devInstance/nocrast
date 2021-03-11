using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NoCrast.Server.Database;
using NoCrast.Server.Model;
using NoCrast.Server.Services;
using NoCrast.Shared.Model;
using NoCrast.Shared.Utils;
using System;
using System.Linq;

namespace NoCrast.Server.Controllers
{
    [ApiController]
    [Route("api/data/reports")]
    public class ReportController : UserBaseController
    {
        private ActivityReportService ActivityService { get; }
        private AggregateReportService AggregateService { get; }

        public ReportController(ApplicationDbContext d,
                                UserManager<ApplicationUser> userManager,
                                ActivityReportService activityService,
                                AggregateReportService aggregateService)
            : base(d, userManager)
        {
            ActivityService = activityService;
            AggregateService = aggregateService;
        }


        [Authorize]
        [HttpGet]
        [Route("aggregate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<ReportItem> GetAggregateReport(int timeoffset, ReportItem.RIType type, DateTime start)
        {
            return HandleWebRequest<ReportItem>(() =>
            {
                return Ok(AggregateService.GetReport(CurrentProfile, timeoffset, type, start));
            });
        }

        [Authorize]
        [HttpGet]
        [Route("activity")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<ReportItem> GetActivityReport(int timeoffset, ReportItem.RIType type, ReportItem.RIMode mode, int? interval, DateTime? start, DateTime? end)
        {
            return HandleWebRequest<ReportItem>(() =>
            {
                return Ok(ActivityService.GetReport(CurrentProfile, timeoffset, type, mode, interval, start));
            });
        }
    }
}
