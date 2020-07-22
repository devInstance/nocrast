using NoCrast.Shared.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoCrast.Client.Services.Api
{
    public interface ITasksApi
    {
        Task<TaskItem[]> GetTasksAsync();

        Task<TaskItem> AddTaskAsync(TaskItem task);

        Task<TaskItem> UpdateTaskAsync(string id, TaskItem task);

        Task<TaskItem> RemoveTaskAsync(string id);

        Task<TimeLogItem[]> GetTimelogAsync(string id);

        Task<UpdateTaskParameters> InsertTimerAsync(string id, UpdateTaskParameters request);

        Task<UpdateTaskParameters> UpdateTimerAsync(string id, string timerId, UpdateTaskParameters request);

        Task<UpdateTaskParameters> RemoveTimerAsync(string id, string timerId);

    }
}
