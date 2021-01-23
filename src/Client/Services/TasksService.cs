using DevInstance.LogScope;
using NoCrast.Client.ModelExtensions;
using NoCrast.Client.Services.Api;
using NoCrast.Shared.Model;
using NoCrast.Shared.Utils;
using System;
using System.Threading.Tasks;

namespace NoCrast.Client.Services
{
    public class TasksService : BaseService
    {
        public ITimeProvider TimeProvider { get; }
        protected ITasksApi TaskApi { get; }
        protected ITagsApi TagsApi { get; }

        public TasksService(NotificationService notification,
                        ITimeProvider provider,
                        ILogProvider logProvider,
                        ITasksApi tasksApi,
                        ITagsApi tagsApi) : base(notification)
        {
            TaskApi = tasksApi;
            TagsApi = tagsApi;
            TimeProvider = provider;
            Log = logProvider.CreateLogger(this);
            Log.D("constructor");
        }

        /*
        private async Task<bool> SyncUpWithServerAsync(List<TaskItem> tasks)
        {
            using (var l = Log.DebugExScope())
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
            using (var l = Log.DebugExScope())
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

        public async Task<TaskItem[]> GetRunningTasksAsync()
        {
            using (var l = Log.DebugExScope())
            {
                ResetUIError();
                try
                {
                    var tasks = await TaskApi.GetTasksAsync(TimeProvider.UtcTimeOffset, null, null, TaskFilter.RunningOnly);

                    ResetNetworkError();

                    return tasks;
                }
                catch (Exception ex)
                {
                    NotifyNetworkError(ex);
                }
                return null;
            }
        }

        public async Task<TaskItem[]> GetTasksForDashboardAsync()
        {
            using (var l = Log.DebugExScope())
            {
                ResetUIError();
                try
                {
                    var tasks = await TaskApi.GetTasksAsync(TimeProvider.UtcTimeOffset, 4, null, TaskFilter.StoppedOnly);

                    ResetNetworkError();

                    return tasks;
                }
                catch (Exception ex)
                {
                    NotifyNetworkError(ex);
                }
                return null;
            }
        }

        public async Task<TaskItem[]> GetTasksAsync(int? top, int? page)
        {
            using (var l = Log.DebugExScope())
            {
                ResetUIError();
                try
                {
                    var tasks = await TaskApi.GetTasksAsync(TimeProvider.UtcTimeOffset, top, page, null);

                    ResetNetworkError();

                    return tasks;
                }
                catch (Exception ex)
                {
                    NotifyNetworkError(ex);
                }
                return null;
            }
        }

        public async Task<TaskItem[]> GetTodayTasksAsync()
        {
            using (var l = Log.DebugExScope())
            {
                ResetUIError();
                try
                {
                    var tasks = await TaskApi.GetTasksAsync(TimeProvider.UtcTimeOffset, null, null, TaskFilter.Today);

                    ResetNetworkError();

                    return tasks;
                }
                catch (Exception ex)
                {
                    NotifyNetworkError(ex);
                }
                return null;
            }
        }

        public async Task<TaskItem> GetTaskAsync(string taskId)
        {
            using (var l = Log.DebugExScope())
            {
                ResetUIError();
                try
                {
                    //TODO: optimize, don't fetch full list, create back local store
                    var task = await TaskApi.GetTaskAsync(taskId, TimeProvider.UtcTimeOffset);

                    ResetNetworkError();

                    return task;
                }
                catch (Exception ex)
                {
                    NotifyNetworkError(ex);
                }
                return null;
            }
        }

        public async Task<TaskItem> AddNewTaskAsync(string title)
        {
            using (var l = Log.DebugExScope())
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
                    Title = title,
                    IsRunning = false
                };
                // Step 4: Post the object on server
                TaskItem response = null;
                try
                {
                    response = await TaskApi.AddTaskAsync(task, TimeProvider.UtcTimeOffset);
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

                return response;
            }
        }

        public async Task<TaskItem> UpdateTaskTitleAsync(TaskItem task, string newTitle)
        {
            using (var l = Log.DebugExScope())
            {
                TaskItem newTask;
                try
                {
                    task.Title = newTitle;
                    newTask = await TaskApi.UpdateTaskAsync(task.Id, task, TimeProvider.UtcTimeOffset);
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

        public async Task<bool> RemoveTaskAsync(TaskItem item)
        {
            using (var l = Log.DebugExScope())
            {
                try
                {
                    await TaskApi.RemoveTaskAsync(item.Id);
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

        public async void StartTaskAsync(TaskItem item)
        {
            using (var l = Log.DebugExScope())
            {
                if (item is null)
                {
                    throw new ArgumentNullException(nameof(item));
                }

                if (item.IsRunning) return;

                item.Start(TimeProvider);

                try
                {
                    item = await TaskApi.InsertTimerAsync(item.Id, true, item.ActiveTimeLogItem, TimeProvider.UtcTimeOffset);
                    ResetNetworkError();
                    NotifyDataHasChanged();
                }
                catch (Exception ex)
                {
                    NotifyNetworkError(ex);
                }

            }
        }

        public async void StopTaskAsync(TaskItem item)
        {
            using (var l = Log.DebugExScope())
            {
                if (item is null)
                {
                    throw new ArgumentNullException(nameof(item));
                }

                if (!item.IsRunning) return;

                item.Stop(TimeProvider);

                try
                {
                    item = await TaskApi.UpdateTimerAsync(item.Id, item.ActiveTimeLogItem.Id, false, item.ActiveTimeLogItem, TimeProvider.UtcTimeOffset);
                    ResetNetworkError();
                    NotifyDataHasChanged();
                }
                catch (Exception ex)
                {
                    NotifyNetworkError(ex);
                }
            }
        }

        public async Task<ModelList<TimeLogItem>> GetTimeLogItemsAsync(TaskItem item, TimeLogResultType type, int? top, int? page)
        {
            using (var l = Log.DebugExScope())
            {
                if (item is null)
                {
                    throw new ArgumentNullException(nameof(item));
                }

                ModelList<TimeLogItem> response = null;
                try
                {
                    response = await TaskApi.GetTimelogAsync(item.Id, TimeProvider.UtcTimeOffset, top, page, type);
                    ResetNetworkError();
                }
                catch (Exception ex)
                {
                    NotifyNetworkError(ex);
                }

                return response;
            }
        }

        public async Task<TaskItem> UpdateTimelogAsync(TaskItem item, TimeLogItem log)
        {
            using (var l = Log.DebugExScope())
            {
                try
                {
                    item = await TaskApi.UpdateTimerAsync(item.Id, log.Id, item.IsRunning, log, TimeProvider.UtcTimeOffset);
                    ResetNetworkError();

                    NotifyDataHasChanged();
                }
                catch (Exception ex)
                {
                    l.E(ex);
                    NotifyNetworkError(ex);
                }
                return item;
            }
        }

        public async Task<TaskItem> InsertTimelogAsync(TaskItem item, TimeLogItem log)
        {
            using (var l = Log.DebugExScope())
            {
                try
                {
                    item = await TaskApi.InsertTimerAsync(item.Id, null, log, TimeProvider.UtcTimeOffset);
                    ResetNetworkError();

                    NotifyDataHasChanged();
                }
                catch (Exception ex)
                {
                    l.E(ex);
                    NotifyNetworkError(ex);
                }
                return item;
            }
        }

        public async Task<TaskItem> RemoveTimelogAsync(TaskItem item, TimeLogItem log)
        {
            using (var l = Log.DebugExScope())
            {
                try
                {
                    var result = await TaskApi.RemoveTimerAsync(item.Id, log.Id, TimeProvider.UtcTimeOffset);
                    ResetNetworkError();
                    //NotifyDataHasChanged();
                    return result;
                }
                catch (Exception ex)
                {
                    l.E(ex);
                    NotifyNetworkError(ex);
                }
                return item;
            }
        }

        public async Task<TagItem[]> GetTagsAsync(TaskItem item)
        {
            using (var l = Log.DebugExScope())
            {
                try
                {
                    var result = await TagsApi.GetTagsByTaskIdAsync(item.Id);
                    ResetNetworkError();
                    return result;
                }
                catch (Exception ex)
                {
                    l.E(ex);
                    NotifyNetworkError(ex);
                }
                return null;
            }
        }

        public async Task<TagItem[]> GetAllTagsAsync()
        {
            using (var l = Log.DebugExScope())
            {
                try
                {
                    var res = await TagsApi.GetTagsAsync(false); //TODO: should be cached

                    ResetNetworkError();

                    return res;
                }
                catch (Exception ex)
                {
                    l.E(ex);
                    NotifyNetworkError(ex);
                }
                return null;
            }
        }

        public async Task<TagItem> AddOrCreateTagAsync(TaskItem item, string name)
        {
            using (var l = Log.DebugExScope())
            {
                var tag = new TagItem
                {
                    Name = name
                };

                try
                {
                    var response = await TagsApi.AddTagAsync(tag);

                    await TagsApi.AddTagTaskAsync(item.Id, response.Id);

                    return response;
                }
                catch (Exception ex)
                {
                    l.E(ex);
                    NotifyNetworkError(ex);
                }
                return null;
            }
        }

        public async Task<bool> AddTagAsync(TaskItem item, string id)
        {
            using (var l = Log.DebugExScope())
            {
                try
                {
                    await TagsApi.AddTagTaskAsync(item.Id, id);

                    return true;
                }
                catch (Exception ex)
                {
                    l.E(ex);
                    NotifyNetworkError(ex);
                }
                return false;
            }
        }

        public async Task<bool> RemoveTagAsync(TaskItem item, string id)
        {
            using (var l = Log.DebugExScope())
            {
                try
                {
                    return await TagsApi.RemoveTagTaskAsync(item.Id, id);
                }
                catch (Exception ex)
                {
                    l.E(ex);
                    NotifyNetworkError(ex);
                }
                return false;
            }
        }

        public async Task<TaskItem> UpdateProjectAsync(TaskItem item, string id)
        {
            using (var l = Log.DebugExScope())
            {
                TaskItem newTask;
                try
                {
                    if(id != null)
                    {
                        item.Project = new ProjectItem { Id = id };
                    }
                    newTask = await TaskApi.UpdateTaskAsync(item.Id, item, TimeProvider.UtcTimeOffset);
                    ResetNetworkError();
                }
                catch (Exception ex)
                {
                    newTask = item;
                    newTask.ClientId = IdGenerator.New();

                    NotifyNetworkError(ex);
                }

                return newTask;
            }
        }
    }
}
