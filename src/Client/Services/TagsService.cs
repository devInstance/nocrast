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

        protected ITagsApi Api { get; }

        public TagsService(ITimeProvider provider,
                            ILogProvider logProvider,
                            ITagsApi api)
        {
            Api = api;
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
                    var response = await Api.GetTagsAsync();
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
                    var response = await Api.GetTagsAsync();
                    ResetNetworkError();
                    return response.Where(f => f.Id == id).FirstOrDefault(); //TODO: Fix me
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
                    var response = await Api.AddTagAsync(tag);
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
                    newTag = await Api.UpdateTagAsync(tag.Id, tag);
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
                    await Api.RemoveTagAsync(item.Id);
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
