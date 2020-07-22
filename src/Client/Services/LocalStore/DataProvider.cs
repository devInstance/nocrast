using Blazored.LocalStorage;
using NoCrast.Client.Model;
using NoCrast.Client.ModelExtensions;
using NoCrast.Shared.Logging;
using NoCrast.Shared.Model;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using System.Linq;
using System;
using NoCrast.Shared.Utils;

namespace NoCrast.Client.Services.LocalStore
{
    public class DataProvider : IDataProvider
    {
        private NoCrastData data = null;

        public event LoadTasksAsync OnLoadTasks;
        public event LoadTimeLogAsync OnLoadTimeLog;

        public ITimeProvider TimeProvider { get; }
        protected ILocalStorageService Storage { get; }
        public ILog Log { get; protected set; }

        public DataProvider(ITimeProvider timeProvider, ILocalStorageService storage, ILogProvider logProvider)
        {
            TimeProvider = timeProvider;
            Storage = storage;
            Log = logProvider.CreateLogger(this);
            Log.D("constructor");
        }

        public async Task<List<TaskItem>> GetTasksAsync()
        {
            await TryLoadDataAsync();

            return data.Tasks;
        }

        private async Task<bool> TryLoadDataAsync()
        {
            if (data == null)
            {
                using (var l = Log.DebugScope())
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

                    // Step 2: If no local data then initialize empty
                    if (data == null)
                    {
                        data = new NoCrastData();
                        data.Tasks = new List<TaskItem>();
                        data.Logs = new List<List<TimeLogItem>>();
                    }

                    // Step 3: Sync-up with server
                    if (OnLoadTasks != null)
                    {
                        data.Tasks = await OnLoadTasks(data.Tasks);
                        data.Logs.Clear();
                        foreach (var task in data.Tasks)
                        {
                            //TODO: Fix it
                            data.Logs.Add(await OnLoadTimeLog(task, new List<TimeLogItem>()));
                        }
                        await SaveDataAsync();
                    }
                }
            }
            return true;
        }

        private async Task SaveDataAsync()
        {
            using (var l = Log.DebugScope())
            {
                await TryLoadDataAsync();

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

        public async Task<TaskItem> FindTaskByTitleAsync(string title)
        {
            await TryLoadDataAsync();
            return (from d in data.Tasks where d.Title == title select d).FirstOrDefault();
        }

        public async Task<TaskItem> CreateTaskAsync(string title)
        {
            await TryLoadDataAsync();

            var task = new TaskItem { Title = title };
            task.ClientId = IdGenerator.New();
            data.Tasks.Add(task);
            data.Logs.Add(new List<TimeLogItem>());

            await SaveDataAsync();

            return task;
        }

        public async Task<bool> UpdateTaskAsync(TaskItem item)
        {
            await TryLoadDataAsync();

            if (data.ApplyTaskItem(item))
            {
                await SaveDataAsync();
                return true;
            }

            return false;
        }

        public async Task<bool> RemoveTaskAsync(TaskItem item)
        {
            await TryLoadDataAsync();

            int index = data.FindTaskIndex(item);
            if (index >= 0)
            {
                data.Tasks.RemoveAt(index);
                data.Logs.RemoveAt(index);
                await SaveDataAsync();

                return true;
            }
            return false;
        }

        public async Task<List<TimeLogItem>> GetTimeLogAsync(TaskItem item)
        {
            await TryLoadDataAsync();

            //TODO: This is a temp solution based on assumption that
            //list is not previously fetched from the server if count is 0
            //It should a flag introduced in the long run 
            //if task has id it has been already synced with server

            var taskIndex = data.FindTaskIndex(item);
            //TODO: proper sync-up
            if (!String.IsNullOrEmpty(item.Id) && data.Logs[taskIndex].Count == 0)
            {
                data.Logs[taskIndex] = await OnLoadTimeLog(item, data.Logs[taskIndex]);
            }

            return new List<TimeLogItem>(data.Logs[taskIndex]);
        }

        public async Task<TimeLogItem> CreateTimeLogAsync(TaskItem item)
        {
            await TryLoadDataAsync();

            TimeLogItem log = new TimeLogItem
            {
                StartTime = TimeProvider.CurrentTime
            };
            log.ClientId = IdGenerator.New();

            int index = data.FindTaskIndex(item);
            if(index < 0)
            {
                throw new Exception("Invalid task");
            }
            data.Logs[index].Insert(0, log);
            return log;
        }

        public async Task<bool> UpdateTimeLogAsync(TaskItem item, TimeLogItem log)
        {
            await TryLoadDataAsync();

            if(data.ApplyStartTaskParameters(item, log))
            {
                await SaveDataAsync();
                return true;
            }

            return false;
        }

        public async Task<bool> RemoveTimeLogAsync(TimeLogItem item)
        {
            await TryLoadDataAsync();

            throw new NotImplementedException();
        }
    }
}
