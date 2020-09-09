using NoCrast.Client.Services.Api;
using NoCrast.Shared.Model;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace NoCrast.Client.Services.Net
{
    public class TagsApi : ApiBase, ITagsApi
    {
        private const string Controller = "api/data/tags/";

        public TagsApi(HttpClient http) : base(http)
        {
        }

        public async Task<TagItem[]> GetTagsAsync(int timeoffset)
        {
            return await httpClient.GetFromJsonAsync<TagItem[]>($"{Controller}");
        }

        public async Task<TagItem> AddTagAsync(TagItem tag, int timeoffset)
        {
            var result = await httpClient.PostAsJsonAsync($"{Controller}", tag);
            result.EnsureSuccessStatusCode();
            return result.Content.ReadFromJsonAsync<TagItem>().Result;
        }

        public async Task<TagItem> UpdateTagAsync(string id, TagItem tag, int timeoffset)
        {
            var result = await httpClient.PutAsJsonAsync($"{Controller}{id}", tag);
            result.EnsureSuccessStatusCode();
            return result.Content.ReadFromJsonAsync<TagItem>().Result;
        }

        public async Task<bool> RemoveTagAsync(string id)
        {
            var result = await httpClient.DeleteAsync($"{Controller}{id}");
            result.EnsureSuccessStatusCode();
            return result.Content.ReadFromJsonAsync<bool>().Result;
        }

    }
}
