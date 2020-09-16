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
    [Route("api/data/tasks")]
    public class TasksController : UserBaseController
    {
        public ITimeProvider TimeProvider { get; }

        public TasksController(ApplicationDbContext d,
                                UserManager<ApplicationUser> userManager,
                                ITimeProvider timeProvider)
            : base(d, userManager)
        {
            TimeProvider = timeProvider;
        }

        private IQueryable<TimerTask> SelectTasks()
        {
            return (from tks in DB.Tasks
                   where tks.Profile == CurrentProfile
                   orderby tks.CreateDate
                   select tks).Include(q => q.State);
        }

        private IQueryable<TaskItem> DecorateTasks(IQueryable<TimerTask> q, int timeoffset)
        {
            DateTime now = TimeProvider.CurrentTime;
            DateTime startOfTheWeek = TimeConverter.GetStartOfTheWeekForTimeOffset(now, timeoffset);
            DateTime startOfTheDay = TimeConverter.GetStartOfTheDayForTimeOffset(now, timeoffset);

            return from tks in q
                   where tks.Profile == CurrentProfile
                   orderby tks.CreateDate
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
                       TimeLogCount = tks.TimeLog.Count,
                       Title = tks.Title,
                       TotalTimeSpent = (from tl in DB.TimeLog
                                         where tl.TaskId == tks.Id
                                         select tl.ElapsedMilliseconds).Sum(),
                       TotalTimeSpentThisWeek = (from tl in DB.TimeLog
                                                 where tl.TaskId == tks.Id
                                                 && tl.StartTime >= startOfTheWeek
                                                 select tl.ElapsedMilliseconds).Sum(),
                       TotalTimeSpentToday = (from tl in DB.TimeLog
                                              where tl.TaskId == tks.Id
                                              && tl.StartTime >= startOfTheDay
                                              select tl.ElapsedMilliseconds).Sum()
                   };
        }


        [Authorize]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<TaskItem[]> GetTasks(int timeoffset)
        {
            return HandleWebRequest<TaskItem[]>(() =>
            { 
                var tasks = DecorateTasks(SelectTasks(), timeoffset).ToList();

                return Ok(tasks.ToArray());
            });
        }

        [Authorize]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<TaskItem> AddTask(TaskItem task)
        {
            return HandleWebRequest<TaskItem>(() =>
            {
                DateTime now = TimeProvider.CurrentTime;

                var newTask = new TimerTask
                {
                    Id = Guid.NewGuid(),
                    Profile = CurrentProfile,
                    PublicId = IdGenerator.New(),

                    Title = task.Title,

                    CreateDate = now,
                    UpdateDate = now
                };

                var newTaskState = new TimerTaskState
                {
                    Task = newTask,
                    IsRunning = task.IsRunning,
                };

                DB.Tasks.Add(newTask);
                DB.TaskState.Add(newTaskState);
                DB.SaveChanges();

                task.Id = newTask.PublicId;

                return Ok(task);
            });

        }

        [Authorize]
        [HttpPut]
        [Route("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<TaskItem> UpdateTask(string id, [FromBody] TaskItem task)
        {
            return HandleWebRequest<TaskItem>(() =>
            {
                DateTime now = TimeProvider.CurrentTime;

                var taskRecord = (from task in DB.Tasks
                                  where task.PublicId == id && task.Profile == CurrentProfile
                                  select task).Include(a => a.State).FirstOrDefault();

                if (taskRecord == null)
                {
                    return NotFound();
                }

                taskRecord.Title = task.Title;
                taskRecord.UpdateDate = now;
                DB.SaveChanges();

                return Ok(task);
            });
        }

        [Authorize]
        [HttpDelete]
        [Route("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<bool> RemoveTask(string id)
        {
            return HandleWebRequest<bool>(() =>
            {
                var taskRecord = (from task in DB.Tasks
                                  where task.PublicId == id && task.Profile == CurrentProfile
                                  select task).Include(a => a.State).FirstOrDefault();

                if (taskRecord == null)
                {
                    return NotFound();
                }

                DB.Tasks.Remove(taskRecord);
                DB.SaveChanges();

                return Ok(true);
            });
        }

        [Authorize]
        [HttpGet]
        [Route("{id}/timelog")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<TimeLogItem[]> GetTimelogAsync(string id)
        {
            return HandleWebRequest<TimeLogItem[]>(() =>
            {
                var result = (from tasks in DB.Tasks
                              join timeLog in DB.TimeLog on tasks equals timeLog.Task
                              where tasks.PublicId == id && tasks.Profile == CurrentProfile
                              orderby timeLog.StartTime descending
                              select new TimeLogItem
                              {
                                  Id = timeLog.PublicId,
                                  ElapsedMilliseconds = timeLog.ElapsedMilliseconds,
                                  StartTime = new DateTime(timeLog.StartTime.Ticks, DateTimeKind.Utc) //TODO: ???
                              }).ToList();
                return Ok(result.ToArray());
            });
        }

        [Authorize]
        [HttpPost]
        [Route("{id}/timelog")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<TaskItem> InsertTimerLogAsync(string id, bool? start, int timeoffset, [FromBody] TimeLogItem request)
        {
            return HandleWebRequest<TaskItem>(() =>
            {
                DateTime now = TimeProvider.CurrentTime;

                var taskRecord = (from task in DB.Tasks
                                  where task.PublicId == id && task.Profile == CurrentProfile
                                  select task).Include(a => a.State).FirstOrDefault();
                if (taskRecord == null)
                {
                    return NotFound();
                }

                var timeLog = new TimeLog
                {
                    Id = Guid.NewGuid(),
                    PublicId = IdGenerator.New(),

                    TaskId = taskRecord.Id,
                    StartTime = request.StartTime,
                    ElapsedMilliseconds = request.ElapsedMilliseconds,

                    CreateDate = now,
                    UpdateDate = now,
                };
                DB.TimeLog.Add(timeLog);

                taskRecord.State.IsRunning = start ?? false;
                taskRecord.UpdateDate = now;
                taskRecord.State.ActiveTimeLogItem = timeLog;

                DB.SaveChanges();

                var response = (from ts in DecorateTasks(SelectTasks(), timeoffset)
                                where ts.Id == taskRecord.PublicId
                                select ts).FirstOrDefault();

                return Ok(response);
            });
        }

        [Authorize]
        [HttpPut]
        [Route("{id}/timelog/{timerId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<TaskItem> UpdateTimerLogAsync(string id, string timerId, bool? start, int timeoffset, [FromBody] TimeLogItem request)
        {
            return HandleWebRequest<TaskItem>(() =>
            {
                DateTime now = TimeProvider.CurrentTime;

                var taskRecord = (from task in DB.Tasks
                                  join timeLog in DB.TimeLog on task equals timeLog.Task
                                  join state in DB.TaskState on task equals state.Task
                                  where task.PublicId == id && task.Profile == CurrentProfile && timeLog.PublicId == timerId
                                  select new { task, timeLog, state }).FirstOrDefault();
                if (taskRecord == null)
                {
                    return NotFound();
                }

                taskRecord.state.IsRunning = start ?? false;
                taskRecord.task.UpdateDate = now;

                taskRecord.timeLog.StartTime = request.StartTime;
                taskRecord.timeLog.ElapsedMilliseconds = request.ElapsedMilliseconds;

                DB.SaveChanges();

                var selectTasks = DecorateTasks(SelectTasks(), timeoffset);

                var updatedTask = (from t in selectTasks
                                   where t.Id == id
                                   select t).FirstOrDefault();

                return Ok(updatedTask);
            });
        }

        [Authorize]
        [HttpDelete]
        [Route("{id}/timelog/{timerId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<TaskItem> DeleteTimerLogAsync(string id, string timerId, int timeoffset)
        {
            return HandleWebRequest<TaskItem>(() =>
            {
                var timeLogRecord = (from task in DB.Tasks
                                     join timeLog in DB.TimeLog on task equals timeLog.Task
                                     where task.PublicId == id && task.Profile == CurrentProfile && timeLog.PublicId == timerId
                                     select timeLog).FirstOrDefault();
                if (timeLogRecord == null)
                {
                    return NotFound();
                }

                DB.TimeLog.Remove(timeLogRecord);
                DB.SaveChanges();

                var selectTasks = DecorateTasks(SelectTasks(), timeoffset);

                var updatedTask = (from t in selectTasks
                                   where t.Id == id
                                   select t).FirstOrDefault();

                return Ok(updatedTask);
            });
        }

        [Authorize]
        [HttpGet]
        [Route("tag/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<TaskItem[]> GetTasksByTagIdAsync(string id, int timeoffset)
        {
            return HandleWebRequest<TaskItem[]>(() =>
            {
                var tasks = SelectTasks();

                var result = (from tsk in tasks
                              join ttt in DB.TagToTimerTasks on tsk equals ttt.Task
                              join tag in DB.TimerTags on ttt.Tag equals tag
                              where tag.PublicId == id && tag.Profile == CurrentProfile
                              select tsk);

                return Ok(DecorateTasks(result, timeoffset).ToArray());
            });
        }
    }
}
