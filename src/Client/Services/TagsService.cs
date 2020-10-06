using NoCrast.Client.Services.Api;
using NoCrast.Shared.Logging;
using NoCrast.Shared.Model;
using NoCrast.Shared.Utils;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace NoCrast.Client.Services
{
    public class TagsService : BaseService
    {
        public ITimeProvider TimeProvider { get; }

        protected ITagsApi TagsApi { get; }
        protected ITasksApi TasksApi { get; }

        public TagsService(ITimeProvider provider,
                            ILogProvider logProvider,
                            ITagsApi tagsApi,
                            ITasksApi tasksApi,
                            NotificationService notificationServ) : base(notificationServ)
        {
            TagsApi = tagsApi;
            TasksApi = tasksApi;
            TimeProvider = provider;
            Log = logProvider.CreateLogger(this);
            Log.D("constructor");
        }

        public async Task<TagItem[]> GetTagsAsync()
        {
            using (var l = Log.DebugScope())
            {
                ResetUIError();
                try
                {
                    var response = await TagsApi.GetTagsAsync(true);
                    ResetNetworkError();
                    return response;
                }
                catch (Exception ex)
                {
                    NotifyNetworkError(ex);
                }
                return null;
            }
        }

        public async Task<TagItem> GetTagAsync(string id)
        {
            using (var l = Log.DebugScope())
            {
                ResetUIError();
                try
                {
                    var response = await TagsApi.GetTagAsync(id);
                    ResetNetworkError();
                    return response;
                }
                catch (Exception ex)
                {
                    NotifyNetworkError(ex);
                }
                return null;
            }
        }

        public async Task<TaskItem[]> GetTasksAsync(string id)
        {
            using (var l = Log.DebugScope())
            {
                ResetUIError();
                try
                {
                    var response = await TasksApi.GetTasksByTagIdAsync(id, TimeProvider.UtcTimeOffset);
                    ResetNetworkError();
                    return response;
                }
                catch (Exception ex)
                {
                    NotifyNetworkError(ex);
                }
                return null;
            }
        }

        public async Task<TagItem> AddNewTagAsync(string newTag)
        {
            using (var l = Log.DebugScope())
            {
                ResetUIError();

                var tag = new TagItem
                {
                    Name = newTag
                };

                try
                {
                    var response = await TagsApi.AddTagAsync(tag);
                    ResetNetworkError();
                    NotifyDataHasChanged();
                    return response;
                }
                catch (Exception ex)
                {
                    NotifyNetworkError(ex);
                }
                return null;
            }
        }

        public async Task<TagItem> UpdateTagNameAsync(TagItem tag, string newName)
        {
            using (var l = Log.DebugScope())
            {
                TagItem newTag;
                try
                {
                    tag.Name = newName;
                    newTag = await TagsApi.UpdateTagAsync(tag.Id, tag);
                    ResetNetworkError();
                }
                catch (Exception ex)
                {
                    newTag = tag;
                    NotifyNetworkError(ex);
                }

                return newTag;
            }
        }

        public async Task<bool> RemoveTagAsync(TagItem item)
        {
            using (var l = Log.DebugScope())
            {
                try
                {
                    await TagsApi.RemoveTagAsync(item.Id);
                    ResetNetworkError();
                    NotifyDataHasChanged();
                    return true;
                }
                catch (Exception ex)
                {
                    NotifyNetworkError(ex);
                    return false;
                }
            }
        }

    }
}
