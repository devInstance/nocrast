using NoCrast.Client.Model;
using NoCrast.Shared.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoCrast.Client.Services.LocalStore
{
    public delegate Task<List<TaskItem>> LoadTasksAsync(List<TaskItem> items);
    public delegate Task<List<TimeLogItem>> LoadTimeLogAsync(TaskItem task, List<TimeLogItem> items);
    public interface IDataProvider
    {
        event LoadTasksAsync OnLoadTasks;
        event LoadTimeLogAsync OnLoadTimeLog;

        Task<List<TaskItem>> GetTasksAsync();

        Task<TaskItem> FindTaskByTitleAsync(string title);

        Task<TaskItem> CreateTaskAsync(string title);

        Task<bool> UpdateTaskAsync(TaskItem item);

        Task<bool> RemoveTaskAsync(TaskItem item);

        Task<List<TimeLogItem>> GetTimeLogAsync(TaskItem item);

        Task<TimeLogItem> CreateTimeLogAsync(TaskItem item);

        Task<bool> UpdateTimeLogAsync(TaskItem item, TimeLogItem log);

        Task<bool> RemoveTimeLogAsync(TaskItem item, TimeLogItem log);
    }
}
