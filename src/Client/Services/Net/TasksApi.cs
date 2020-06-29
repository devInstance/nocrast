using NoCrast.Client.Services.Api;
using NoCrast.Shared.Model;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace NoCrast.Client.Services.Net
{
    public class TasksApi : ApiBase, ITasksApi
    {
        private const string Controller = "api/data/tasks/";

        public TasksApi(HttpClient http) : base(http)
        {
        }

        public async Task<TaskItem> AddTaskAsync(TaskItem task)
        {
            var result = await httpClient.PostAsJsonAsync(Controller, task);
            result.EnsureSuccessStatusCode();
            return result.Content.ReadFromJsonAsync<TaskItem>().Result;
        }

        public Task<TaskItem[]> GetTasksAsync()
        {
            return httpClient.GetFromJsonAsync<TaskItem[]>(Controller);
        }

        public async Task<TaskItem> RemoveTaskAsync(string id)
        {
            var result = await httpClient.DeleteAsync(Controller + id);
            result.EnsureSuccessStatusCode();
            return result.Content.ReadFromJsonAsync<TaskItem>().Result;
        }

        public Task<UpdateTaskParameters> UpdateTimerAsync(UpdateTaskParameters request)
        {
            throw new System.NotImplementedException();
        }

        public Task<UpdateTaskParameters> StopTimerAsync(UpdateTaskParameters request)
        {
            throw new System.NotImplementedException();
        }

        public async Task<TaskItem[]> SyncUpWithServer(TaskItem[] tasks)
        {
            var result = await httpClient.PostAsJsonAsync(Controller + "sync-up", tasks);
            result.EnsureSuccessStatusCode();
            return result.Content.ReadFromJsonAsync<TaskItem[]>().Result;
        }

        public async Task<TaskItem> UpdateTaskAsync(TaskItem task)
        {
            var result = await httpClient.PutAsJsonAsync(Controller + task.Id, task);
            result.EnsureSuccessStatusCode();
            return result.Content.ReadFromJsonAsync<TaskItem>().Result;
        }
    }
}
