using NoCrast.Shared.Model;
using System.Threading.Tasks;

namespace NoCrast.Client.Services.Api
{
    public interface IProjectsApi
    {
        Task<ProjectItem[]> GetProjectsAsync();

        Task<ProjectItem> GetProjectAsync(string id);

        Task<ProjectItem> AddProjectAsync(ProjectItem tag);

        Task<ProjectItem> UpdateProjectAsync(string id, ProjectItem tag);

        Task<bool> RemoveProjectAsync(string id);

        Task<ProjectItem> AddTaskToProjectAsync(string projectId, string taskId);

        Task<bool> RemoveTaskToProjectAsync(string projectId, string taskId);
    }
}
