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

        Task<TaskItem> RemoveTaskAsync(string id);

        Task<TaskItem> UpdateTaskAsync(TaskItem task);

        Task<TaskItem[]> SyncUpWithServer(TaskItem[] tasks);
    }
}
