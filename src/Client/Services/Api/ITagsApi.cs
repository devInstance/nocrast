using NoCrast.Shared.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoCrast.Client.Services.Api
{
    public interface ITagsApi
    {
        Task<TagItem[]> GetTagsAsync(int timeoffset);

        Task<TagItem> AddTagAsync(TagItem tag, int timeoffset);

        Task<TagItem> UpdateTagAsync(string id, TagItem task, int timeoffset);

        Task<bool> RemoveTagAsync(string id);
    }
}
