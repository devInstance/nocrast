using Blazored.LocalStorage;
using NoCrast.Client.Model;
using NoCrast.Client.ModelExtensions;
using NoCrast.Client.ModelViews;
using NoCrast.Client.Services.Api;
using NoCrast.Client.Utils;
using NoCrast.Shared.Logging;
using NoCrast.Shared.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
            Log.D("contructor");
        }

        private async Task<NoCrastData> TryLoadDataAsync()
        {
            if (data == null)
            {
                data = await Storage.GetItemAsync<NoCrastData>(NoCrastData.StorageKeyName);

                SyncUpWithServer();

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
                    var timeLog = data.Logs[i].Find(f => f.Id == task.LatestTimeLogItemId);
                    var itemView = new TaskItemView(Provider, task, timeLog);
                }
                return result;
            }
        }

        public async Task<TaskItemView> AddNewTaskAsync(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                throw new ArgumentException("Empty title", nameof(title));
            }

            await TryLoadDataAsync();

            if ((from d in data.Tasks where d.Title == title select d).FirstOrDefault() != null)
            {
                NotifyUIError(new ArgumentException("Task already exists", nameof(title)));
                return null;
            }

            ResetUIError();

            var task = new TaskItem { Title = title };
            task.SetInternalId(Guid.NewGuid());
            data.Tasks.Add(task);
            data.Logs.Add(new List<TimeLogItem>());

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

            if (response != null)
            {
                if (!data.ApplyTaskItem(task, response))
                {
                    await LocalDataOverideAsync();
                }
            }

            await SaveDataAsync();

            NotifyDataHasChanged();

            return new TaskItemView(Provider, response, null);
        }

        /// <summary>
        /// Should be called after app start
        /// </summary>
        public async void SyncUpWithServer()
        {
            if (data != null)
            {
                try
                {
                    //TODO: only sync-up if you have internal id
                    var result = await Api.SyncUpWithServer(data.Tasks.ToArray());
                    data.Tasks.Clear();
                    data.Tasks.AddRange(result);
                    ResetNetworkError();
                }
                catch (Exception ex)
                {
                    NotifyNetworkError(ex);
                }
            }
        }

        private async Task SaveDataAsync()
        {
            await Storage.SetItemAsync(NoCrastData.StorageKeyName, data);
        }

        public async Task<bool> RemoveTaskAsync(TaskItemView item)
        {
            await TryLoadDataAsync();

            int index = data.Tasks.FindIndex(f => f.Id == item.Task.Id);
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

        public async void StartTaskAsync(TaskItemView item)
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
            log.SetInternalId(Guid.NewGuid());

            item.TimeLog = log;
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

            if(response != null)
            {
                if (!data.ApplyStartTaskParameters(request, response))
                {
                    await LocalDataOverideAsync();
                }
            }

            await SaveDataAsync();

            NotifyDataHasChanged();
        }

        private Task LocalDataOverideAsync()
        {
            throw new NotImplementedException();
        }

        public async void StopTaskAsync(TaskItemView item)
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (!item.Task.IsRunning) return;

            var load = TryLoadDataAsync();

            item.Stop();

            await load;

            int index = data.Tasks.FindIndex(f => f.Id == item.Task.Id);
            if (index >= 0)
            {
                data.Tasks[index] = item.Task;
                int indexLog = data.Logs[index].FindIndex(f => f.Id == item.TimeLog.Id);
                if (indexLog >= 0)
                {
                    data.Logs[index][indexLog] = item.TimeLog;
                }
            }
            else
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
                if (!data.ApplyStartTaskParameters(request, response))
                {
                    await LocalDataOverideAsync();
                }
            }

            await SaveDataAsync();

            NotifyDataHasChanged();
        }

        public async Task<List<TimeLogItem>> GetTimeLogItemsAsync(TaskItem item)
        {
            throw new NotImplementedException();
        }
    }
}
