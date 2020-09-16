using NoCrast.Shared.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoCrast.Client.Services.Api
{
    public interface ITagsApi
    {
        Task<TagItem[]> GetTagsAsync();

        Task<TagItem> GetTagAsync(string id);

        Task<TagItem> AddTagAsync(TagItem tag);

        Task<TagItem> UpdateTagAsync(string id, TagItem tag);

        Task<bool> RemoveTagAsync(string id);

        Task<TagItem[]> GetTagsByTaskIdAsync(string id);

        Task<TagItem> AddTagTaskAsync(string taskId, string tagId);

        Task<bool> RemoveTagTaskAsync(string taskId, string tagId);

    }
}
