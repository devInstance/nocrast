using Blazored.LocalStorage;
using NoCrast.Client.Model;
using NoCrast.Client.ModelExtensions;
using NoCrast.Client.Services.Api;
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
        protected ILocalStorageService Storage { get; }
        public ILog Log { get; private set; }
        protected ITasksApi Api { get; }

        public event EventHandler DataHasChanged;
        public event ServiceErrorEventHandler ErrorHasOccured;

        private bool isNetworkErrorRisen = false;
        private bool isUiErrorRisen = false;

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

        private void NotifyDataHasChanged()
        {
            if (DataHasChanged != null)
            {
                DataHasChanged(this, new EventArgs());
            }
        }

        private void NotifyUIError(Exception ex)
        {
            var arg = new ServiceErrorEventArgs()
            {
                Message = ex.Message,
                IsUIError = true
            };

            Log.E(ex);
            NotifyError(arg);
            isUiErrorRisen = true;
        }

        private void ResetUIError()
        {
            if (isUiErrorRisen)
            {
                var arg = new ServiceErrorEventArgs()
                {
                    ResetUIError = true
                };
                NotifyError(arg);
                isUiErrorRisen = false;
            }
        }

        private void ResetNetworkError()
        {
            if (isNetworkErrorRisen)
            {
                var arg = new ServiceErrorEventArgs()
                {
                    ResetNetworkError = true
                };
                NotifyError(arg);
                isNetworkErrorRisen = false;
            }
        }

        private void NotifyNetworkError(Exception ex)
        {
            var arg = new ServiceErrorEventArgs()
            {
                Message = ex.Message,
                IsNetworkError = true
            };

            Log.E(ex);
            NotifyError(arg);
            isNetworkErrorRisen = true;
        }

        private void NotifyError(ServiceErrorEventArgs arg)
        {
            if (ErrorHasOccured != null)
            {
                ErrorHasOccured(this, arg);
            }
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

        public async Task<List<TaskItem>> GetTasksAsync()
        {
            using (var l = Log.DebugScope())
            {
                ResetUIError();
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
                NotifyUIError(new ArgumentException("Task already exists", nameof(title)));
                return null;
                //throw;
            }
            ResetUIError();

            var task = new TaskItem { Title = title };
            data.Tasks.Add(task);

            try
            {
                task = await Api.AddTaskAsync(task);
                ResetNetworkError();
            }
            catch (Exception ex)
            {
                NotifyNetworkError(ex);
                //fallback
                //If server is not availabe, generate id
            }

            await SaveDataAsync();

            NotifyDataHasChanged();

            return task;
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

        public async Task<bool> RemoveTaskAsync(TaskItem item)
        {
            await TryLoadDataAsync();

            if(data.Tasks.Remove(item))
            {
                try
                {
                    await Api.RemoveTaskAsync(item.Id);
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

        public async void StartTaskAsync(TaskItem item)
        {
            await TryLoadDataAsync();

            item.Start(Provider);

            try
            {
                await Api.UpdateTaskAsync(item);
                ResetNetworkError();
            }
            catch (Exception ex)
            {
                NotifyNetworkError(ex);
            }

            await SaveDataAsync();

            NotifyDataHasChanged();
        }

        public async void StopTaskAsync(TaskItem item)
        {
            await TryLoadDataAsync();

            item.Stop(Provider);

            try
            {
                await Api.UpdateTaskAsync(item);
                ResetNetworkError();
            }
            catch (Exception ex)
            {
                NotifyNetworkError(ex);
            }

            await SaveDataAsync();

            NotifyDataHasChanged();
        }
    }
}
