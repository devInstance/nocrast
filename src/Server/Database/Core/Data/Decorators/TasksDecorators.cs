using NoCrast.Server.Data.Queries;
using NoCrast.Server.Model;
using NoCrast.Shared.Model;
using NoCrast.Shared.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoCrast.Server.Database.Data.Decorators
{
    public static class TasksDecorators
    {
        public static List<TaskItem> SelectView(this ITasksQuery query)
        {
            return (from tks in query.Select()
                   select new TaskItem
                   {
                       Id = tks.PublicId,
                       IsRunning = tks.State.IsRunning,
                       ActiveTimeLogItem = tks.State.ActiveTimeLogItem != null ? new TimeLogItem
                       {
                           Id = tks.State.ActiveTimeLogItem.PublicId,
                           ElapsedMilliseconds = tks.State.ActiveTimeLogItem.ElapsedMilliseconds,
                           StartTime = new DateTime(tks.State.ActiveTimeLogItem.StartTime.Ticks, DateTimeKind.Utc) //TODO: ???
                       } : null,
                       UpdateDate = tks.UpdateDate,
                       TimeLogCount = tks.TimeLog.Count,
                       Title = tks.Title,
                       Project = tks.Project != null ? new ProjectItem { Id = tks.Project.PublicId, Title = tks.Project.Title, Descritpion = tks.Project.Descritpion, Color = tks.Project.Color } : null,
                       Descritpion = tks.Descritpion
                   }).ToList();
        }

        //public static List<TaskItem> With(this List<TaskItem> list, ITimeLogQuery qtime, ITimeProvider provider, int timeoffset)
        //{
        //    DateTime now = provider.CurrentTime;
        //    DateTime startOfTheWeek = TimeConverter.GetStartOfTheWeekForTimeOffset(now, timeoffset);
        //    DateTime startOfTheDay = TimeConverter.GetStartOfTheDayForTimeOffset(now, timeoffset);

        //    return from l in list
        //           select new TaskItem
        //           {
        //               TotalTimeSpent = (from tl in qtime
        //                                 where tl.TaskId == tks.Id
        //                                 select tl.ElapsedMilliseconds).Sum(),
        //               TotalTimeSpentThisWeek = (from tl in qtime
        //                                         where tl.TaskId == tks.Id
        //                                         && tl.StartTime >= startOfTheWeek
        //                                         select tl.ElapsedMilliseconds).Sum(),
        //               TotalTimeSpentToday = (from tl in qtime
        //                                      where tl.TaskId == tks.Id
        //                                      && tl.StartTime >= startOfTheDay
        //                                      select tl.ElapsedMilliseconds).Sum()
        //           };

        //}
    }
}
