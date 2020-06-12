using Blazored.LocalStorage;
using NoCrast.Client.Model;
using NoCrast.Client.ModelExtensions;
using NoCrast.Client.Utils;
using NoCrast.Shared.Logging;
using NoCrast.Shared.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoCrast.Client.Services
{
    public class TimersService
    {
        private NoCrastData data = null;

        public ITimeProvider Provider { get; }
        public ILocalStorageService Storage { get; }
        public ILog Log { get; private set; }

        public event EventHandler DataHasChanged;

        public TimersService(ITimeProvider provider, ILogProvider logProvider, ILocalStorageService storage)
        {
            Provider = provider;
            Storage = storage;
            Log = logProvider.CreateLogger(this);
            Log.D("contructor");
        }

        private void NotifyDataHasChanged()
        {
            if (DataHasChanged != null)
            {
                DataHasChanged(this, new EventArgs());
            }
        }

        private async Task<NoCrastData> TryLoadDataAsync()
        {
            if (data == null)
            {
                data = await Storage.GetItemAsync<NoCrastData>(NoCrastData.StorageKeyName);
                if (data == null)
                {
                    data = new NoCrastData();
                }
            }
            return data;
        }

        public async Task<List<TaskItem>> GetTasksAsync()
        {
            using (var l = Log.DebugScope())
            {
                await TryLoadDataAsync();
                return data.Tasks;
            }
        }

        public async Task<TaskItem> AddNewTaskAsync(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                throw new ArgumentException("Empty title", nameof(title));
            }

            await TryLoadDataAsync();

            if ((from d in data.Tasks where d.Title == title select d).FirstOrDefault() != null)
            {
                throw new ArgumentException("Task already exists", nameof(title));
            }

            var task = new TaskItem { Title = title };
            data.Tasks.Add(task);

            await SaveDataAsync();

            NotifyDataHasChanged();

            return task;
        }

        private async Task SaveDataAsync()
        {
            await Storage.SetItemAsync(NoCrastData.StorageKeyName, data);
        }

        public async Task<bool> RemoveTaskAsync(TaskItem item)
        {
            await TryLoadDataAsync();

            if(data.Tasks.Remove(item))
            {
                await SaveDataAsync();

                NotifyDataHasChanged();
                return true;
            }

            return false;
        }

        public async void StartTaskAsync(TaskItem item)
        {
            await TryLoadDataAsync();

            item.Start(Provider);
            await SaveDataAsync();

            NotifyDataHasChanged();
        }

        public async void StopTaskAsync(TaskItem item)
        {
            await TryLoadDataAsync();

            item.Stop(Provider);
            await SaveDataAsync();

            NotifyDataHasChanged();
        }
    }
}
