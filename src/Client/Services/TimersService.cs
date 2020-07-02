using Blazored.LocalStorage;
using NoCrast.Client.Model;
using NoCrast.Client.ModelExtensions;
using NoCrast.Client.ModelViews;
using NoCrast.Client.Services.Api;
using NoCrast.Client.Utils;
using NoCrast.Shared.Logging;
using NoCrast.Shared.Model;
using PhotoShaRa.Lib.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace NoCrast.Client.Services
{
    public class TimersService : BaseService
    {
        private NoCrastData data = null;

        public ITimeProvider Provider { get; }
        protected ILocalStorageService Storage { get; }
        protected ITasksApi Api { get; }

        public TimersService(ITimeProvider provider,
                            ILogProvider logProvider,
                            ILocalStorageService storage, 
                            ITasksApi api)
        {
            Api = api;
            Provider = provider;
            Storage = storage;
            Log = logProvider.CreateLogger(this);
            Log.D("constructor");
        }

        private async Task<NoCrastData> TryLoadDataAsync()
        {
            using (var l = Log.DebugScope())
            {
                if (data == null)
                {
                    // Step 1: Try loading from the local store
                    data = await Storage.GetItemAsync<NoCrastData>(NoCrastData.StorageKeyName);

                    JsonSerializerOptions options = new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        WriteIndented = true
                    };

                    string result = JsonSerializer.Serialize<NoCrastData>(data, options);
                    l.D(result);

                    // Step 2: Sync-up with server any previous un-committed changes
                    await SyncUpWithServer();

                    // Step 3: If no local data and server is not responding then initialize empty
                    if (data == null)
                    {
                        data = new NoCrastData();
                        try
                        {
                            var tasks = await Api.GetTasksAsync();
                            data.Tasks.AddRange(tasks);
                            ResetNetworkError();
                        }
                        catch (Exception ex)
                        {
                            NotifyNetworkError(ex);
                        }
                    }
                }
                return data;
            }
        }

        public async Task<List<TaskItemView>> GetTasksAsync()
        {
            using (var l = Log.DebugScope())
            {
                ResetUIError();
                await TryLoadDataAsync();

                var result = new List<TaskItemView>();
                for(int i = 0; i < data.Tasks.Count; i ++)
                {
                    var task = data.Tasks[i];
                    var logs = data.Logs[i];
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
                    var itemView = new TaskItemView(Provider, task, lastTimeLog, totalTime);
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

                // Step 1: Make sure data is already loaded
                await TryLoadDataAsync();

                // Step 2: Validate integrity
                if ((from d in data.Tasks where d.Title == title select d).FirstOrDefault() != null)
                {
                    NotifyUIError(new ArgumentException("Task already exists", nameof(title)));
                    return null;
                }

                ResetUIError();

                // Step 3: Create a new object
                var task = new TaskItem { Title = title };
                task.ClientId = IdGenerator.New();
                data.Tasks.Add(task);
                data.Logs.Add(new List<TimeLogItem>());

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
                    if (!data.ApplyTaskItem(response))
                    {
                        // if response cannot be applied then local data is 
                        // corrupted and full re-sync from server required
                        await LocalDataOverideAsync();
                    }
                }

                // Step 6: Save to the local storage
                await SaveDataAsync();

                NotifyDataHasChanged();

                return new TaskItemView(Provider, response, null, 0);
            }
        }

        /// <summary>
        /// Should be called after app start
        /// </summary>
        public async Task<bool> SyncUpWithServer()
        {
            using (var l = Log.DebugScope())
            {
                bool result = false;
                if (data != null)
                {
                    try
                    {
                        //TODO: only sync-up if you have internal id
                        var tasks = await Api.SyncUpWithServer(data.Tasks.ToArray());
                        //TODO: Proper sync-up
                        data.Tasks.Clear();
                        data.Tasks.AddRange(tasks);
                        ResetNetworkError();
                        result = true;
                    }
                    catch (Exception ex)
                    {
                        NotifyNetworkError(ex);
                    }
                }
                return result;
            }
        }

        private async Task SaveDataAsync()
        {
            using (var l = Log.DebugScope())
            {
                JsonSerializerOptions options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                };

                string result = JsonSerializer.Serialize<NoCrastData>(data, options);
                l.D(result);
                await Storage.SetItemAsync(NoCrastData.StorageKeyName, data);
            }
        }

        public async Task<bool> RemoveTaskAsync(TaskItemView item)
        {
            using (var l = Log.DebugScope())
            {
                await TryLoadDataAsync();

                int index = data.FindTaskIndex(item.Task);
                if (index >= 0)
                {
                    data.Tasks.RemoveAt(index);
                    data.Logs.RemoveAt(index);
                    try
                    {
                        await Api.RemoveTaskAsync(item.Task.Id);
                        ResetNetworkError();
                    }
                    catch (Exception ex)
                    {
                        NotifyNetworkError(ex);
                    }

                    await SaveDataAsync();

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

                var load = TryLoadDataAsync();

                TimeLogItem log = new TimeLogItem
                {
                    StartTime = Provider.CurrentTime
                };
                log.ClientId = IdGenerator.New();

                item.TimeLog = log;
                item.Task.LatestTimeLogItemId = log.ClientId;
                item.Task.IsRunning = true;

                await load;

                if (!data.InsertNewLog(item.Task, log))
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
                    if (!data.ApplyStartTaskParameters(response))
                    {
                        await LocalDataOverideAsync();
                    }
                }

                await SaveDataAsync();

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

                await TryLoadDataAsync();

                item.Stop();

                if (!data.UpdateTimeLog(item.Task, item.TimeLog))
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
                    if (!data.ApplyStartTaskParameters(response))
                    {
                        await LocalDataOverideAsync();
                    }
                }

                await SaveDataAsync();

                NotifyDataHasChanged();
            }
        }

        public async Task<List<TimeLogItem>> GetTimeLogItemsAsync(TaskItem item)
        {
            throw new NotImplementedException();
        }
    }
}
