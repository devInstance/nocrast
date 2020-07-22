using NoCrast.Client.Services.Api;
using NoCrast.Shared.Model;
using System.Collections.Generic;
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

        public async Task<TaskItem[]> GetTasksAsync()
        {
            return await httpClient.GetFromJsonAsync<TaskItem[]>(Controller);
        }

        public async Task<TaskItem> AddTaskAsync(TaskItem task)
        {
            var result = await httpClient.PostAsJsonAsync(Controller, task);
            result.EnsureSuccessStatusCode();
            return result.Content.ReadFromJsonAsync<TaskItem>().Result;
        }

        public async Task<TaskItem> UpdateTaskAsync(string id, TaskItem task)
        {
            var result = await httpClient.PutAsJsonAsync(Controller + id, task);
            result.EnsureSuccessStatusCode();
            return result.Content.ReadFromJsonAsync<TaskItem>().Result;
        }

        public async Task<TaskItem> RemoveTaskAsync(string id)
        {
            var result = await httpClient.DeleteAsync(Controller + id);
            result.EnsureSuccessStatusCode();
            return result.Content.ReadFromJsonAsync<TaskItem>().Result;
        }

        public async Task<TimeLogItem[]> GetTimelogAsync(string id)
        {
            return await httpClient.GetFromJsonAsync<TimeLogItem[]>(Controller + id + "/timelog");
        }

        public async Task<UpdateTaskParameters> InsertTimerAsync(string id, UpdateTaskParameters request)
        {
            var result = await httpClient.PostAsJsonAsync(Controller + id + "/timelog", request);
            result.EnsureSuccessStatusCode();
            return result.Content.ReadFromJsonAsync<UpdateTaskParameters>().Result;
        }

        public async Task<UpdateTaskParameters> UpdateTimerAsync(string id, string timerId, UpdateTaskParameters request)
        {
            var result = await httpClient.PutAsJsonAsync(Controller + id + "/timelog/" + timerId, request);
            result.EnsureSuccessStatusCode();
            return result.Content.ReadFromJsonAsync<UpdateTaskParameters>().Result;
        }

        public async Task<UpdateTaskParameters> RemoveTimerAsync(string id, string timerId)
        {
            var result = await httpClient.DeleteAsync(Controller + id + "/timelog/" + timerId);
            result.EnsureSuccessStatusCode();
            return result.Content.ReadFromJsonAsync<UpdateTaskParameters>().Result;
        }
    }
}
