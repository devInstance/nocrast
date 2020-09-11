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

        Task<TagItem> AddTagAsync(TagItem tag);

        Task<TagItem> UpdateTagAsync(string id, TagItem task);

        Task<bool> RemoveTagAsync(string id);
    }
}
