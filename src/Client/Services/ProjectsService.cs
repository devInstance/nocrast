using DevInstance.LogScope;
using NoCrast.Client.Services.Api;
using NoCrast.Shared.Model;
using NoCrast.Shared.Utils;
using System;
using System.Threading.Tasks;

namespace NoCrast.Client.Services
{
    public class ProjectsService : BaseService
    {
        public ITimeProvider TimeProvider { get; }

        protected IProjectsApi ProjectsApi { get; }
        protected ITasksApi TasksApi { get; }

        public ProjectsService(ITimeProvider provider,
                            IScopeManager logProvider,
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

        public async Task<ProjectItem[]> GetProjectsAsync(bool addTotals)
        {
            using (var l = Log.DebugExScope())
            {
                ResetUIError();
                try
                {
                    var response = await ProjectsApi.GetProjectsAsync(addTotals);
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
            using (var l = Log.DebugExScope())
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
            using (var l = Log.DebugExScope())
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

        public async Task<ProjectItem> AddNewProjectAsync(string newTitle, string newDescription, ProjectColor color)
        {
            using (var l = Log.DebugExScope())
            {
                ResetUIError();

                var project = new ProjectItem
                {
                    Title = newTitle,
                    Descritpion = newDescription,
                    Color = color
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

        public async Task<ProjectItem> UpdateProjectNameAsync(ProjectItem project, string newTitle)
        {
            using (var l = Log.DebugExScope())
            {
                ProjectItem newProject;
                try
                {
                    project.Title = newTitle;
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

        public async Task<ProjectItem> UpdateProjectAsync(ProjectItem project)
        {
            using (var l = Log.DebugExScope())
            {
                ProjectItem newProject;
                try
                {
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

        public async Task<ProjectItem> UpdateProjectDescriptionAsync(ProjectItem project, string newDescription)
        {
            using (var l = Log.DebugExScope())
            {
                project.Descritpion = newDescription;
                return await UpdateProjectAsync(project);
            }
        }

        public async Task<ProjectItem> UpdateProjectColorAsync(ProjectItem project, ProjectColor color)
        {
            using (var l = Log.DebugExScope())
            {
                project.Color = color;
                return await UpdateProjectAsync(project);
            }
        }

        public async Task<bool> RemoveProjectAsync(ProjectItem item)
        {
            using (var l = Log.DebugExScope())
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
