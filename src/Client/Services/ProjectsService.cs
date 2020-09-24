using NoCrast.Client.Services.Api;
using NoCrast.Shared.Logging;
using NoCrast.Shared.Model;
using NoCrast.Shared.Utils;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace NoCrast.Client.Services
{
    public class ProjectsService : BaseService
    {
        public ITimeProvider TimeProvider { get; }

        protected IProjectsApi ProjectsApi { get; }
        protected ITasksApi TasksApi { get; }

        public ProjectsService(ITimeProvider provider,
                            ILogProvider logProvider,
                            IProjectsApi projectsApi,
                            ITasksApi tasksApi,
                            NotificationService notificationServ) : base(notificationServ)
        {
            ProjectsApi = projectsApi;
            TasksApi = tasksApi;
            TimeProvider = provider;
            Log = logProvider.CreateLogger(this);
            Log.D("constructor");
        }

        public async Task<ProjectItem[]> GetProjectsAsync()
        {
            using (var l = Log.DebugScope())
            {
                ResetUIError();
                try
                {
                    var response = await ProjectsApi.GetProjectsAsync();
                    ResetNetworkError();
                    return response;
                }
                catch (Exception ex)
                {
                    NotifyNetworkError(ex);
                }
                return null;
            }
        }

        public async Task<ProjectItem> GetProjectAsync(string id)
        {
            using (var l = Log.DebugScope())
            {
                ResetUIError();
                try
                {
                    var response = await ProjectsApi.GetProjectAsync(id);
                    ResetNetworkError();
                    return response;
                }
                catch (Exception ex)
                {
                    NotifyNetworkError(ex);
                }
                return null;
            }
        }

        public async Task<TaskItem[]> GetTasksAsync(string id)
        {
            using (var l = Log.DebugScope())
            {
                ResetUIError();
                try
                {
                    var response = await TasksApi.GetTasksByProjectIdAsync(id, TimeProvider.UtcTimeOffset);
                    ResetNetworkError();
                    return response;
                }
                catch (Exception ex)
                {
                    NotifyNetworkError(ex);
                }
                return null;
            }
        }

        public async Task<ProjectItem> AddNewProjectAsync(string newTitle, string newDescription)
        {
            using (var l = Log.DebugScope())
            {
                ResetUIError();

                var project = new ProjectItem
                {
                    Title = newTitle,
                    Descritpion = newDescription
                };

                try
                {
                    var response = await ProjectsApi.AddProjectAsync(project);
                    ResetNetworkError();
                    NotifyDataHasChanged();
                    return response;
                }
                catch (Exception ex)
                {
                    NotifyNetworkError(ex);
                }
                return null;
            }
        }

        public async Task<ProjectItem> UpdateProjectAsync(ProjectItem project, string newTitle, string newDescription)
        {
            using (var l = Log.DebugScope())
            {
                ProjectItem newProject;
                try
                {
                    project.Title = newTitle;
                    project.Descritpion = newDescription;
                    newProject = await ProjectsApi.UpdateProjectAsync(project.Id, project);
                    ResetNetworkError();
                }
                catch (Exception ex)
                {
                    newProject = project;
                    NotifyNetworkError(ex);
                }

                return newProject;
            }
        }

        public async Task<bool> RemoveProjectAsync(ProjectItem item)
        {
            using (var l = Log.DebugScope())
            {
                try
                {
                    await ProjectsApi.RemoveProjectAsync(item.Id);
                    ResetNetworkError();
                    NotifyDataHasChanged();
                    return true;
                }
                catch (Exception ex)
                {
                    NotifyNetworkError(ex);
                    return false;
                }
            }
        }

    }
}
