using NoCrast.Client.Services.Api;
using NoCrast.Shared.Model;
using System;
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

        public async Task<TaskItem[]> GetTasksAsync(int timeoffset)
        {
            return await httpClient.GetFromJsonAsync<TaskItem[]>($"{Controller}?timeoffset={timeoffset}");
        }

        public async Task<TaskItem> AddTaskAsync(TaskItem task, int timeoffset)
        {
            var result = await httpClient.PostAsJsonAsync($"{Controller}?timeoffset={timeoffset}", task);
            result.EnsureSuccessStatusCode();
            return result.Content.ReadFromJsonAsync<TaskItem>().Result;
        }

        public async Task<TaskItem> UpdateTaskAsync(string id, TaskItem task, int timeoffset)
        {
            var result = await httpClient.PutAsJsonAsync($"{Controller}{id}?timeoffset={timeoffset}", task);
            result.EnsureSuccessStatusCode();
            return result.Content.ReadFromJsonAsync<TaskItem>().Result;
        }

        public async Task<bool> RemoveTaskAsync(string id)
        {
            var result = await httpClient.DeleteAsync($"{Controller}{id}");
            result.EnsureSuccessStatusCode();
            return result.Content.ReadFromJsonAsync<bool>().Result;
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

        public async Task<UpdateTaskParameters> UpdateTimerAsync(string id, string timerId, UpdateTaskParameters request, int timeoffset)
        {
            var result = await httpClient.PutAsJsonAsync($"{Controller}{id}/timelog/{timerId}?timeoffset={timeoffset}", request);
            result.EnsureSuccessStatusCode();
            return result.Content.ReadFromJsonAsync<UpdateTaskParameters>().Result;
        }

        public async Task<TaskItem> RemoveTimerAsync(string id, string timerId, int timeoffset)
        {
            var result = await httpClient.DeleteAsync($"{Controller}{id}/timelog/{timerId}?timeoffset={timeoffset}");
            result.EnsureSuccessStatusCode();
            return result.Content.ReadFromJsonAsync<TaskItem>().Result;
        }
    }
}
