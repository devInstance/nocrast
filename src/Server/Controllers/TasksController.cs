using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NoCrast.Server.Database;
using NoCrast.Server.Model;
using NoCrast.Shared.Model;
using NoCrast.Shared.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
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
                            join st in DB.TaskState on tasks equals st.Task
                            where tasks.Profile == CurrentProfile
                            select new TaskItem
                            {
                                Id = tasks.PublicId,
                                IsRunning = st.IsRunning,
                                LatestTimeLogItemId = st.LatestTimeLogItem != null ? st.LatestTimeLogItem.PublicId : null,
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

        public ActionResult<TaskItem> RemoveTask(string id)
        {
            return HandleWebRequest<TaskItem>(() =>
            {
                throw new NotImplementedException();
            });
        }
        /*
        Task<TaskItem> UpdateTaskAsync(TaskItem task)
        {

        }

        Task<UpdateTaskParameters> UpdateTimerAsync(UpdateTaskParameters request)
        {

        }

        Task<List<TimeLogItem>> GetTimelogAsync(string taskid)
        {

        }
        */
    }
}
