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
        public TasksController(ApplicationDbContext d, UserManager<ApplicationUser> userManager) 
            : base(d, userManager)
        {
        }

        private IQueryable<TaskItem> SelectTasks()
        {
            DateTime startOfTheWeek = DateTime.Now.StartOfWeek(DayOfWeek.Monday);

            return from tks in DB.Tasks
                   join state in DB.TaskState on tks equals state.Task
                   where tks.Profile == CurrentProfile
                   select new TaskItem
                   {
                       Id = tks.PublicId,
                       IsRunning = state.IsRunning,
                       ActiveTimeLogItemId = state.ActiveTimeLogItem != null ? state.ActiveTimeLogItem.PublicId : null,
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
                                              && tl.StartTime >= DateTime.Now.Date
                                              select tl.ElapsedMilliseconds).Sum()
                   };
        }

        [Authorize]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<TaskItem[]> GetTasks()
        {
            return HandleWebRequest<TaskItem[]>(() =>
            {

                var tasks = SelectTasks().ToList();

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
                var newTask = new TimerTask
                {
                    Id = Guid.NewGuid(),
                    Profile = CurrentProfile,
                    PublicId = IdGenerator.New(),

                    Title = task.Title,

                    CreateDate = DateTime.Now,//TODO: ITimeProvider ???
                    UpdateDate = DateTime.Now,//TODO: ITimeProvider ???
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
                var taskRecord = (from task in DB.Tasks
                                  where task.PublicId == id && task.Profile == CurrentProfile
                                  select task).Include(a => a.State).FirstOrDefault();

                if(taskRecord == null)
                {
                    return NotFound();
                }

                taskRecord.Title = task.Title;
                taskRecord.UpdateDate = DateTime.Now;
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
                                  StartTime = new DateTime(timeLog.StartTime.Ticks, DateTimeKind.Utc)
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
        public ActionResult<UpdateTaskParameters> InsertTimerLogAsync(string id, [FromBody] UpdateTaskParameters request)
        {
            return HandleWebRequest<UpdateTaskParameters>(() =>
            {
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
                    StartTime = request.Log.StartTime,
                    ElapsedMilliseconds = request.Log.ElapsedMilliseconds,

                    CreateDate = DateTime.Now,//TODO: ITimeProvider ???
                    UpdateDate = DateTime.Now,//TODO: ITimeProvider ???
                };
                DB.TimeLog.Add(timeLog);

                taskRecord.State.IsRunning = request.Task.IsRunning;
                taskRecord.UpdateDate = DateTime.Now;
                taskRecord.State.ActiveTimeLogItem = timeLog;

                DB.SaveChanges();

                request.Log.Id = timeLog.PublicId;
                request.Task.Id = taskRecord.PublicId;
                request.Task.ActiveTimeLogItemId = timeLog.PublicId;

                return Ok(request);
            });
        }

        [Authorize]
        [HttpPut]
        [Route("{id}/timelog/{timerId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<UpdateTaskParameters> UpdateTimerLogAsync(string id, string timerId, [FromBody] UpdateTaskParameters request)
        {
            return HandleWebRequest<UpdateTaskParameters>(() =>
            {
                var taskRecord = (from task in DB.Tasks
                                  join timeLog in DB.TimeLog on task equals timeLog.Task
                                  join state in DB.TaskState on task equals state.Task
                                  where task.PublicId == id && task.Profile == CurrentProfile && timeLog.PublicId == timerId
                                  select new { task, timeLog, state }).FirstOrDefault();
                if (taskRecord == null)
                {
                    return NotFound();
                }

                taskRecord.state.IsRunning = request.Task.IsRunning;
                taskRecord.task.UpdateDate = DateTime.Now;

                taskRecord.timeLog.StartTime = request.Log.StartTime;
                taskRecord.timeLog.ElapsedMilliseconds = request.Log.ElapsedMilliseconds;

                DB.SaveChanges();

                return Ok(request);
            });
        }

        [Authorize]
        [HttpDelete]
        [Route("{id}/timelog/{timerId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<TaskItem> DeleteTimerLogAsync(string id, string timerId)
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

                var selectTasks = SelectTasks();

                var updatedTask = (from t in selectTasks
                                   where t.Id == id
                                   select t).FirstOrDefault();

                return Ok(updatedTask);
            });
        }
    }
}
