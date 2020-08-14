using NoCrast.Shared.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoCrast.Client.Services.Api
{
    public interface ITasksApi
    {
        Task<TaskItem[]> GetTasksAsync(int timeoffset);

        Task<TaskItem> AddTaskAsync(TaskItem task, int timeoffset);

        Task<TaskItem> UpdateTaskAsync(string id, TaskItem task, int timeoffset);

        Task<bool> RemoveTaskAsync(string id);

        Task<TimeLogItem[]> GetTimelogAsync(string id);

        Task<TaskItem> InsertTimerAsync(string id, bool startTask, TimeLogItem log, int timeoffset);

        Task<TaskItem> UpdateTimerAsync(string id, string timerId, bool startTask, TimeLogItem log, int timeoffset);

        Task<TaskItem> RemoveTimerAsync(string id, string timerId, int timeoffset);

    }
}
