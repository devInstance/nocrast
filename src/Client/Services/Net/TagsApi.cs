using NoCrast.Client.Services.Api;
using NoCrast.Shared.Model;
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

        public async Task<TagItem[]> GetTagsAsync(bool addTotals)
        {
            return await httpClient.GetFromJsonAsync<TagItem[]>($"{Controller}?addTotals={addTotals}");
        }

        public async Task<TagItem> GetTagAsync(string id)
        {
            return await httpClient.GetFromJsonAsync<TagItem>($"{Controller}{id}");
        }

        public async Task<TagItem> AddTagAsync(TagItem tag)
        {
            var result = await httpClient.PostAsJsonAsync($"{Controller}", tag);
            result.EnsureSuccessStatusCode();
            return result.Content.ReadFromJsonAsync<TagItem>().Result;
        }

        public async Task<TagItem> UpdateTagAsync(string id, TagItem tag)
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

        public async Task<TagItem[]> GetTagsByTaskIdAsync(string id)
        {
            return await httpClient.GetFromJsonAsync<TagItem[]>($"{Controller}task/{id}");
        }

        public async Task<TagItem> AddTagTaskAsync(string taskId, string tagId)
        {
            var result = await httpClient.PostAsJsonAsync($"{Controller}{tagId}/task", taskId);
            result.EnsureSuccessStatusCode();
            return result.Content.ReadFromJsonAsync<TagItem>().Result;
        }

        public async Task<bool> RemoveTagTaskAsync(string taskId, string tagId)
        {
            var result = await httpClient.DeleteAsync($"{Controller}{tagId}/task/{taskId}");
            result.EnsureSuccessStatusCode();
            return result.Content.ReadFromJsonAsync<bool>().Result;
        }
    }
}
