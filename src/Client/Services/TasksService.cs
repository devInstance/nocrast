using NoCrast.Client.ModelViews;
using NoCrast.Client.Services.Api;
using NoCrast.Shared.Logging;
using NoCrast.Shared.Model;
using NoCrast.Shared.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoCrast.Client.Services
{
    public class TasksService : BaseService
    {
        public ITimeProvider TimeProvider { get; }
        protected ITasksApi Api { get; }

        public TasksService(ITimeProvider provider,
                            ILogProvider logProvider,
                            ITasksApi api)
        {
            Api = api;
            TimeProvider = provider;
            Log = logProvider.CreateLogger(this);
            Log.D("constructor");
        }

        /*
        private async Task<bool> SyncUpWithServerAsync(List<TaskItem> tasks)
        {
            using (var l = Log.DebugScope())
            {
                //Sync first in case there are any pending changes
                foreach (var t in tasks)
                {
                    TaskItem updatedTask = t;
                    if (!String.IsNullOrEmpty(t.ClientId))
                    {
                        //insert
                        if (String.IsNullOrEmpty(t.Id))
                        {
                            l.D($"Insert task {t.Title}");
                            updatedTask = await Api.AddTaskAsync(t, TimeProvider.UtcTimeOffset);
                        }
                        else
                        {
                            l.D($"Update task {t.Title}");
                            updatedTask = await Api.UpdateTaskAsync(t.Id, t, TimeProvider.UtcTimeOffset);
                        }
                        updatedTask.ClientId = null;
                    }
                }
            }
            return true;
        }
        */

        /*
        private async Task<List<TaskItem>> LocalStorage_OnLoadTasks(List<TaskItem> items)
        {
            using (var l = Log.DebugScope())
            {
                List<TaskItem> tasks = items;
                if (items != null)
                {
                    try
                    {
                        //TODO: only sync-up if you have client id
                        await SyncUpWithServerAsync(items);
                        tasks.Clear();
                        tasks.AddRange(await Api.GetTasksAsync(TimeProvider.UtcTimeOffset));
                        ResetNetworkError();
                    }
                    catch (Exception ex)
                    {
                        NotifyNetworkError(ex);
                    }
                }
                return tasks;
            }
        }
        */

        public async Task<List<TaskItemView>> GetTasksAsync()
        {
            using (var l = Log.DebugScope())
            {
                ResetUIError();
                try
                {
                    var tasks = await Api.GetTasksAsync(TimeProvider.UtcTimeOffset);

                    ResetNetworkError();

                    var result = new List<TaskItemView>();
                    for (int i = 0; i < tasks.Length; i++)
                    {
                        var task = tasks[i];
                        var itemView = new TaskItemView(TimeProvider, task);
                        result.Add(itemView);
                    }
                    return result;
                }
                catch (Exception ex)
                {
                    NotifyNetworkError(ex);
                }
                return null;
            }
        }

        public async Task<TaskItemView> GetTaskAsync(string taskId)
        {
            using (var l = Log.DebugScope())
            {
                ResetUIError();
                try
                {

                    //TODO: optimize, don't fetch full list, create back local store
                    var tasks = await Api.GetTasksAsync(TimeProvider.UtcTimeOffset);

                    ResetNetworkError();

                    var task = (from t in tasks where t.Id == taskId select t).FirstOrDefault();
                    if (task == null)
                    {
                        NotifyUIError($"Cannot find task with id {taskId}");
                        return null;
                    }
                    return new TaskItemView(TimeProvider, task);
                }
                catch (Exception ex)
                {
                    NotifyNetworkError(ex);
                }
                return null;
            }
        }

        public async Task<TaskItemView> AddNewTaskAsync(string title)
        {
            using (var l = Log.DebugScope())
            {
                if (string.IsNullOrWhiteSpace(title))
                {
                    throw new ArgumentException("Empty title", nameof(title));
                }

                // Step 2: Validate integrity
                /*if ((await LocalStorage.FindTaskByTitleAsync(title)) != null)
                {
                    NotifyUIError(new ArgumentException("Task already exists", nameof(title)));
                    return null;
                }*/

                ResetUIError();

                // Step 3: Create a new object
                //var task = await LocalStorage.CreateTaskAsync(title);
                var task = new TaskItem
                {
                    Title = title
                };
                // Step 4: Post the object on server
                TaskItem response = null;
                try
                {
                    response = await Api.AddTaskAsync(task, TimeProvider.UtcTimeOffset);
                    ResetNetworkError();
                }
                catch (Exception ex)
                {
                    NotifyNetworkError(ex);
                }

                // Step 5: Apply processed object from server back
                //if (response != null)
                //{
                //    if (!await LocalStorage.UpdateTaskAsync(response))
                //    {
                //        // if response cannot be applied then local data is 
                //        // corrupted and full re-sync from server required
                //        await LocalDataOverideAsync();
                //    }
                //}

                NotifyDataHasChanged();

                return new TaskItemView(TimeProvider, response);
            }
        }

        public async Task<TaskItem> UpdateTaskTitleAsync(TaskItem task, string newTitle)
        {
            using (var l = Log.DebugScope())
            {
                TaskItem newTask;
                try
                {
                    task.Title = newTitle;
                    newTask = await Api.UpdateTaskAsync(task.Id, task, TimeProvider.UtcTimeOffset);
                    ResetNetworkError();
                }
                catch (Exception ex)
                {
                    newTask = task;
                    newTask.ClientId = IdGenerator.New();

                    NotifyNetworkError(ex);
                }

                return newTask;
            }
        }

        public async Task<bool> RemoveTaskAsync(TaskItemView item)
        {
            using (var l = Log.DebugScope())
            {
                try
                {
                    await Api.RemoveTaskAsync(item.Task.Id);
                    ResetNetworkError();
                }
                catch (Exception ex)
                {
                    NotifyNetworkError(ex);
                    return false;
                }
                NotifyDataHasChanged();
                return true;
            }
        }

        public async void StartTaskAsync(TaskItemView item)
        {
            using (var l = Log.DebugScope())
            {
                if (item is null)
                {
                    throw new ArgumentNullException(nameof(item));
                }

                if (item.Task.IsRunning) return;

                item.Start();

                try
                {
                    item.Task = await Api.InsertTimerAsync(item.Task.Id, true, item.Task.ActiveTimeLogItem, TimeProvider.UtcTimeOffset);
                    ResetNetworkError();
                    NotifyDataHasChanged();
                }
                catch (Exception ex)
                {
                    NotifyNetworkError(ex);
                }

            }
        }

        public async void StopTaskAsync(TaskItemView item)
        {
            using (var l = Log.DebugScope())
            {
                if (item is null)
                {
                    throw new ArgumentNullException(nameof(item));
                }

                if (!item.Task.IsRunning) return;

                item.Stop();

                try
                {
                    item.Task = await Api.UpdateTimerAsync(item.Task.Id, item.Task.ActiveTimeLogItem.Id, false, item.Task.ActiveTimeLogItem, TimeProvider.UtcTimeOffset);
                    ResetNetworkError();
                    NotifyDataHasChanged();
                }
                catch (Exception ex)
                {
                    NotifyNetworkError(ex);
                }
            }
        }

        public async Task<List<TimeLogItem>> GetTimeLogItemsAsync(TaskItemView item)
        {
            using (var l = Log.DebugScope())
            {
                if (item is null)
                {
                    throw new ArgumentNullException(nameof(item));
                }
                List<TimeLogItem> response = new List<TimeLogItem>();
                try
                {
                    response.AddRange(await Api.GetTimelogAsync(item.Task.Id));
                    ResetNetworkError();
                }
                catch (Exception ex)
                {
                    NotifyNetworkError(ex);
                }

                return response;
            }
        }

        public async Task<TaskItem> UpdateTimelogAsync(TaskItemView item, TimeLogItem log)
        {
            using (var l = Log.DebugScope())
            {
                try
                {
                    item.Task = await Api.UpdateTimerAsync(item.Task.Id, log.Id, item.Task.IsRunning, log, TimeProvider.UtcTimeOffset);
                    ResetNetworkError();

                    NotifyDataHasChanged();
                }
                catch (Exception ex)
                {
                    l.E(ex);
                    NotifyNetworkError(ex);
                }
                return item.Task;
            }
        }

        public async Task<TaskItem> RemoveTimelogAsync(TaskItemView item, TimeLogItem log)
        {
            using (var l = Log.DebugScope())
            {
                try
                {
                    var result = await Api.RemoveTimerAsync(item.Task.Id, log.Id, TimeProvider.UtcTimeOffset);
                    ResetNetworkError();
                    NotifyDataHasChanged();
                    return result;
                }
                catch (Exception ex)
                {
                    l.E(ex);
                    NotifyNetworkError(ex);
                }
                return item.Task;
            }
        }

    }
}
