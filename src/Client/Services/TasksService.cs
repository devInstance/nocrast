using NoCrast.Client.ModelExtensions;
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

        public async Task<TaskItem[]> GetTasksAsync()
        {
            using (var l = Log.DebugScope())
            {
                ResetUIError();
                try
                {
                    var tasks = await TaskApi.GetTasksAsync(TimeProvider.UtcTimeOffset);

                    ResetNetworkError();
                    NotifyNetworkError(new Exception("test"));

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
            using (var l = Log.DebugScope())
            {
                ResetUIError();
                try
                {

                    //TODO: optimize, don't fetch full list, create back local store
                    var tasks = await TaskApi.GetTasksAsync(TimeProvider.UtcTimeOffset);

                    ResetNetworkError();

                    var task = (from t in tasks where t.Id == taskId select t).FirstOrDefault();
                    if (task == null)
                    {
                        NotifyUIError($"Cannot find task with id {taskId}");
                        return null;
                    }
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
            using (var l = Log.DebugScope())
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
            using (var l = Log.DebugScope())
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
            using (var l = Log.DebugScope())
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
            using (var l = Log.DebugScope())
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

        public async Task<List<TimeLogItem>> GetTimeLogItemsAsync(TaskItem item)
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
                    response.AddRange(await TaskApi.GetTimelogAsync(item.Id));
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
            using (var l = Log.DebugScope())
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

        public async Task<TaskItem> RemoveTimelogAsync(TaskItem item, TimeLogItem log)
        {
            using (var l = Log.DebugScope())
            {
                try
                {
                    var result = await TaskApi.RemoveTimerAsync(item.Id, log.Id, TimeProvider.UtcTimeOffset);
                    ResetNetworkError();
                    NotifyDataHasChanged();
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
            using (var l = Log.DebugScope())
            {
                try
                {
                    var result = await TagsApi.GetTagsByTaskIdAsync(item.Id);
                    ResetNetworkError();
                    NotifyDataHasChanged();
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

        public async Task<TagItem[]> GetNotAssignedTagsAsync(TagItem[] exclude)
        {
            using (var l = Log.DebugScope())
            {
                try
                {
                    var tags = await TagsApi.GetTagsAsync(); //TODO: should be cached

                    ResetNetworkError();

                    //TODO: re-write this code
                    var result = new List<TagItem>(tags);
                    foreach (var ex in exclude)
                    {
                        for(int i = 0; i < result.Count; i ++)
                        {
                            if (result[i].Id == ex.Id)
                            {
                                result.RemoveAt(i);
                            }
                        }
                    }

                    NotifyDataHasChanged();
                    return result.ToArray();
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
            using (var l = Log.DebugScope())
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
            using (var l = Log.DebugScope())
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


        public async Task<TagItem> RemoveTagAsync(TaskItem item, TagItem tag)
        {
            using (var l = Log.DebugScope())
            {
                try
                {
                    return await TagsApi.AddTagTaskAsync(item.Id, tag.Id);
                }
                catch (Exception ex)
                {
                    l.E(ex);
                    NotifyNetworkError(ex);
                }
                return tag;
            }
        }
    }
}
