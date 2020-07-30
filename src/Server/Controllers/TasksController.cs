﻿using Microsoft.AspNetCore.Authorization;
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

        public TasksController(ApplicationDbContext d, UserManager<ApplicationUser> userManager, ITimeProvider timeProvider) 
            : base(d, userManager)
        {
            TimeProvider = timeProvider;
        }

        private IQueryable<TaskItem> SelectTasks(int timeoffset)
        {
            DateTime now = TimeProvider.CurrentTime;
            DateTime startOfTheWeek = now.StartOfWeek(DayOfWeek.Monday).AddMinutes(timeoffset * -1);
            DateTime startOfTheDay = now.Date.AddMinutes(timeoffset * -1);

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
                var tasks = SelectTasks(timeoffset).ToList();

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

                if(taskRecord == null)
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
        public ActionResult<UpdateTaskParameters> InsertTimerLogAsync(string id, [FromBody] UpdateTaskParameters request)
        {
            return HandleWebRequest<UpdateTaskParameters>(() =>
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
                    StartTime = request.Log.StartTime,
                    ElapsedMilliseconds = request.Log.ElapsedMilliseconds,

                    CreateDate = now,
                    UpdateDate = now,
                };
                DB.TimeLog.Add(timeLog);

                taskRecord.State.IsRunning = request.Task.IsRunning;
                taskRecord.UpdateDate = now;
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

                taskRecord.state.IsRunning = request.Task.IsRunning;
                taskRecord.task.UpdateDate = now;

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

                var selectTasks = SelectTasks(timeoffset);

                var updatedTask = (from t in selectTasks
                                   where t.Id == id
                                   select t).FirstOrDefault();

                return Ok(updatedTask);
            });
        }
    }
}
