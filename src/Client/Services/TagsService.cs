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
                    var response = await Api.GetTagsAsync(TimeProvider.UtcTimeOffset);
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
                    var response = await Api.AddTagAsync(tag, TimeProvider.UtcTimeOffset);
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
    }
}
