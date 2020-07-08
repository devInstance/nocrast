using Blazored.LocalStorage;
using NoCrast.Client.Model;
using NoCrast.Client.ModelExtensions;
using NoCrast.Client.ModelViews;
using NoCrast.Client.Services.Api;
using NoCrast.Client.Services.LocalStore;
using NoCrast.Client.Utils;
using NoCrast.Shared.Logging;
using NoCrast.Shared.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace NoCrast.Client.Services
{
    public class TasksService : BaseService
    {
        public ITimeProvider TimeProvider { get; }
        protected IDataProvider LocalStorage { get; }
        protected ITasksApi Api { get; }

        public TasksService(ITimeProvider provider,
                            ILogProvider logProvider,
                            IDataProvider storage,
                            ITasksApi api)
        {
            Api = api;
            TimeProvider = provider;
            LocalStorage = storage;
            Log = logProvider.CreateLogger(this);
            Log.D("constructor");

            LocalStorage.OnLoadTasks += LocalStorage_OnLoadTasks;
            LocalStorage.OnLoadTimeLog += LocalStorage_OnLoadTimeLog;
        }

        private async Task<List<TaskItem>> LocalStorage_OnLoadTasks(List<TaskItem> items)
        {
            List<TaskItem> tasks = items;
            if (items != null)
            {
                try
                {
                    //TODO: only sync-up if you have client id
                    var response = await Api.SyncUpWithServer(items.ToArray());
                    tasks.Clear();
                    tasks.AddRange(response);
                    ResetNetworkError();
                }
                catch (Exception ex)
                {
                    NotifyNetworkError(ex);
                }
            }
            return tasks;
        }

        public async Task<List<TaskItemView>> GetTasksAsync()
        {
            using (var l = Log.DebugScope())
            {
                ResetUIError();

                ///TODO: This is not going to work with server request.
                ///Use Task.TotalTimeSpent instead
                var tasks = await LocalStorage.GetTasksAsync();
                var result = new List<TaskItemView>();
                for (int i = 0; i < tasks.Count; i++)
                {
                    var task = tasks[i];
                    var logs = await LocalStorage.GetTimeLogAsync(task);
                    TimeLogItem lastTimeLog = null;
                    long totalTime = 0;
                    for (int j = 0; j < logs.Count; j++)
                    {
                        var log = logs[j];
                        totalTime += log.ElapsedMilliseconds;
                        if (log.Id == task.LatestTimeLogItemId || log.ClientId == task.LatestTimeLogItemId)
                        {
                            lastTimeLog = log;
                        }
                    }
                    var itemView = new TaskItemView(TimeProvider, task, lastTimeLog, totalTime);
                    result.Add(itemView);
                }
                return result;
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
                if ((await LocalStorage.FindTaskByTitleAsync(title)) != null)
                {
                    NotifyUIError(new ArgumentException("Task already exists", nameof(title)));
                    return null;
                }

                ResetUIError();

                // Step 3: Create a new object
                var task = await LocalStorage.CreateTaskAsync(title);

                // Step 4: Post the object on server
                TaskItem response = null;
                try
                {
                    response = await Api.AddTaskAsync(task);
                    ResetNetworkError();
                }
                catch (Exception ex)
                {
                    NotifyNetworkError(ex);
                }

                // Step 5: Apply processed object from server back
                if (response != null)
                {
                    if (!await LocalStorage.UpdateTaskAsync(response))
                    {
                        // if response cannot be applied then local data is 
                        // corrupted and full re-sync from server required
                        await LocalDataOverideAsync();
                    }
                }

                NotifyDataHasChanged();

                return new TaskItemView(TimeProvider, response, null, 0);
            }
        }

        public async Task<bool> RemoveTaskAsync(TaskItemView item)
        {
            using (var l = Log.DebugScope())
            {
                if (await LocalStorage.RemoveTaskAsync(item.Task))
                {
                    try
                    {
                        await Api.RemoveTaskAsync(item.Task.Id);
                        ResetNetworkError();
                    }
                    catch (Exception ex)
                    {
                        NotifyNetworkError(ex);
                    }
                    NotifyDataHasChanged();

                    return true;

                }

                return false;
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

                TimeLogItem log = await LocalStorage.CreateTimeLogAsync(item.Task);
                item.TimeLog = log;
                item.Task.LatestTimeLogItemId = log.ClientId;
                item.Task.IsRunning = true;

                if (!await LocalStorage.UpdateTimeLogAsync(item.Task, log))
                {
                    await LocalDataOverideAsync();
                }

                var request = new UpdateTaskParameters()
                {
                    Task = item.Task,
                    Log = item.TimeLog
                };

                UpdateTaskParameters response = null;
                try
                {
                    response = await Api.UpdateTimerAsync(request);
                    ResetNetworkError();
                }
                catch (Exception ex)
                {
                    NotifyNetworkError(ex);
                }

                if (response != null)
                {
                    if (!await LocalStorage.UpdateTimeLogAsync(response.Task, response.Log))
                    {
                        await LocalDataOverideAsync();
                    }
                }

                NotifyDataHasChanged();
            }
        }

        private async Task<bool> LocalDataOverideAsync()
        {
            throw new NotImplementedException();
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

                if (!await LocalStorage.UpdateTimeLogAsync(item.Task, item.TimeLog))
                {
                    await LocalDataOverideAsync();
                }

                var request = new UpdateTaskParameters()
                {
                    Task = item.Task,
                    Log = item.TimeLog
                };

                UpdateTaskParameters response = null;
                try
                {
                    response = await Api.UpdateTimerAsync(request);
                    ResetNetworkError();
                }
                catch (Exception ex)
                {
                    NotifyNetworkError(ex);
                }

                if (response != null)
                {
                    if (!await LocalStorage.UpdateTimeLogAsync(response.Task, response.Log))
                    {
                        await LocalDataOverideAsync();
                    }
                }

                NotifyDataHasChanged();
            }
        }

        public async Task<List<TimeLogItem>> GetTimeLogItemsAsync(TaskItemView item, int topn)
        {
            using (var l = Log.DebugScope())
            {
                if (item is null)
                {
                    throw new ArgumentNullException(nameof(item));
                }
                //TODO: This is a temp solution based on assumption that
                //list is not previously fetched from the server if count is 0
                //It should a flag introduced in the long run 
                //if task has id it has been already synced with server
                var list = await LocalStorage.GetTimeLogAsync(item.Task);
                if(topn > 0)
                {
                    return list.Take(topn).ToList();
                }
                return list;
            }
        }

        private async Task<List<TimeLogItem>> LocalStorage_OnLoadTimeLog(TaskItem task, List<TimeLogItem> items)
        {
            List<TimeLogItem> response = null;
            try
            {
                response = await Api.GetTimelogAsync(task.Id);
                ResetNetworkError();
            }
            catch (Exception ex)
            {
                NotifyNetworkError(ex);
            }

            return response;
        }

    }
}
