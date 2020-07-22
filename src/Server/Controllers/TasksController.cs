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
using System.Threading;
using System.Threading.Tasks;

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

        [Authorize]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<TaskItem[]> GetTasks()
        {
            return HandleWebRequest<TaskItem[]>(() =>
            {
                var result = (from tasks in DB.Tasks
                            join state in DB.TaskState on tasks equals state.Task
                            where tasks.Profile == CurrentProfile
                            select new TaskItem
                            {
                                Id = tasks.PublicId,
                                IsRunning = state.IsRunning,
                                LatestTimeLogItemId = state.LatestTimeLogItem != null ? state.LatestTimeLogItem.PublicId : null,
                                TimeLogCount = tasks.TimeLog.Count, //TODO: May not work
                                Title = tasks.Title,
                                //TotalTimeSpent //TODO:
                            }
                            ).ToList();
                return Ok(result.ToArray());
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

                taskRecord.Title = task.Title;
                taskRecord.UpdateDate = DateTime.Now;

                return Ok(task);
            });
        }

        [Authorize]
        [HttpDelete]
        [Route("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<TaskItem> RemoveTask(string id)
        {
            return HandleWebRequest<TaskItem>(() =>
            {
                throw new NotImplementedException();
            });
        }

        [Authorize]
        [HttpGet]
        [Route("{id}/timelog")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
                                  StartTime = timeLog.StartTime
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
                taskRecord.State.LatestTimeLogItem = timeLog;

                DB.SaveChanges();

                request.Log.Id = timeLog.PublicId;
                request.Task.Id = taskRecord.PublicId;
                request.Task.LatestTimeLogItemId = timeLog.PublicId;

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
        public ActionResult<UpdateTaskParameters> DeleteTimerLogAsync(string id, string timerId)
        {
            return HandleWebRequest<UpdateTaskParameters>(() =>
            {
                throw new NotImplementedException();
            });
        }


    }
}
