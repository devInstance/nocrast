using NoCrast.Client.Services.Api;
using NoCrast.Shared.Model;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace NoCrast.Client.Services.Net
{
    public class ProjectsApi : ApiBase, IProjectsApi
    {
        private const string Controller = "api/data/projects/";

        public ProjectsApi(HttpClient http) : base(http)
        {
        }

        public async Task<ProjectItem[]> GetProjectsAsync(bool addTotals)
        {
            return await httpClient.GetFromJsonAsync<ProjectItem[]>($"{Controller}?addTotals={addTotals}");
        }

        public async Task<ProjectItem> GetProjectAsync(string id)
        {
            return await httpClient.GetFromJsonAsync<ProjectItem>($"{Controller}{id}");
        }

        public async Task<ProjectItem> AddProjectAsync(ProjectItem tag)
        {
            var result = await httpClient.PostAsJsonAsync($"{Controller}", tag);
            result.EnsureSuccessStatusCode();
            return result.Content.ReadFromJsonAsync<ProjectItem>().Result;
        }

        public async Task<ProjectItem> UpdateProjectAsync(string id, ProjectItem tag)
        {
            var result = await httpClient.PutAsJsonAsync($"{Controller}{id}", tag);
            result.EnsureSuccessStatusCode();
            return result.Content.ReadFromJsonAsync<ProjectItem>().Result;
        }

        public async Task<bool> RemoveProjectAsync(string id)
        {
            var result = await httpClient.DeleteAsync($"{Controller}{id}");
            result.EnsureSuccessStatusCode();
            return result.Content.ReadFromJsonAsync<bool>().Result;
        }
    }
}
