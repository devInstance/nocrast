using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NoCrast.Server.Database;
using NoCrast.Server.Model;
using NoCrast.Shared.Model;
using NoCrast.Shared.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NoCrast.Server.Controllers
{
    [ApiController]
    [Route("api/data/projects")]
    public class ProjectsController : UserBaseController
    {
        public ITimeProvider TimeProvider { get; }

        public ProjectsController(ApplicationDbContext d,
                                UserManager<ApplicationUser> userManager,
                                ITimeProvider timeProvider)
            : base(d, userManager)
        {
            TimeProvider = timeProvider;
        }

        [Authorize]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<ProjectItem[]> GetProjects(bool addTotals)
        {
            return HandleWebRequest((WebHandler<ProjectItem[]>)(() =>
            {
                List<ProjectItem> result = DecorateProjectItems(SelectProjects(null), addTotals);
                return Ok(result.ToArray());
            }));
        }

        private List<ProjectItem> DecorateProjectItems(IQueryable<Project> q, bool addTotals)
        {
            if(!addTotals)
            {
                return (from project in q
                        select new ProjectItem
                        {
                            Id = project.PublicId,
                            Title = project.Title,
                            Descritpion = project.Descritpion,
                            Color = project.Color,
                            TasksCount = 0,
                            TotalTimeSpent = 0
                        }).ToList();
            }

            var sums = from project in q
                         join task in DB.Tasks on project equals task.Project
                         join tl in DB.TimeLog on task equals tl.Task
                         group tl by new { tl.TaskId, project.Id } into res
                         select new
                         {
                             TaskId = res.Key.TaskId,
                             ProjectId = res.Key.Id,
                             ElapsedMilliseconds = res.Sum(d => d.ElapsedMilliseconds),
                         };

            var tasksSums = from s in sums
                            group s by s.ProjectId into res
                            select new
                            {
                                ProjectId = res.Key,
                                TasksCount = res.Count(),
                                TotalTimeSpent = res.Sum(r => r.ElapsedMilliseconds),
                            };

            var result = from p in q
                         join t in tasksSums on p.Id equals t.ProjectId
                         into l
                         from s in l.DefaultIfEmpty()
                         select new ProjectItem
                         {
                             Id = p.PublicId,
                             Title = p.Title,
                             Descritpion = p.Descritpion,
                             Color = p.Color,
                             TasksCount = s != null ? s.TasksCount : 0,
                             TotalTimeSpent = s != null ? s.TotalTimeSpent: 0
                         };

            return result.ToList();

        }

        private IQueryable<Project> SelectProjects(string id)
        {
            return (from project in DB.Projects
                    where project.Profile == CurrentProfile && (id == null || project.PublicId == id)
                    orderby project.Title ascending
                    select project);
        }

        private IQueryable<Project> SelectProjectsByName(string title)
        {
            return (from projects in DB.Projects
                    where projects.Profile == CurrentProfile && projects.Title == title
                    orderby projects.Title ascending
                    select projects);
        }

        [Authorize]
        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<ProjectItem> GetProject(string id)
        {
            return HandleWebRequest((WebHandler<ProjectItem>)(() =>
            {
                List<ProjectItem> result = DecorateProjectItems(SelectProjects(id), true);
                return Ok(result.FirstOrDefault());
            }));
        }

        [Authorize]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<ProjectItem> AddProject(ProjectItem item)
        {
            return HandleWebRequest<ProjectItem>(() =>
            {
                DateTime now = TimeProvider.CurrentTime;

                //check if the tag with such name already exists
                var existingRecord = SelectProjectsByName(item.Title).FirstOrDefault();
                if(existingRecord != null)
                {
                    return new ProjectItem
                    {
                        Id = existingRecord.PublicId,
                        Title = existingRecord.Title,
                        Descritpion = existingRecord.Descritpion,
                        Color = existingRecord.Color
                    };
                }

                var projectRecord = new Project
                {
                    Id = Guid.NewGuid(),
                    PublicId = IdGenerator.New(),

                    Title = item.Title,
                    Profile = CurrentProfile,
                    Descritpion = item.Descritpion,
                    Color = item.Color,

                    CreateDate = now,
                    UpdateDate = now,
                };
                DB.Projects.Add(projectRecord);

                DB.SaveChanges();

                var response = DecorateProjectItems(SelectProjects(projectRecord.PublicId), false).FirstOrDefault();

                return Ok(response);
            });
        }

        [Authorize]
        [HttpPut]
        [Route("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<ProjectItem> UpdateProject(string id, [FromBody] ProjectItem project)
        {
            return HandleWebRequest<ProjectItem>(() =>
            {
                var projectRecord = SelectProjects(id).FirstOrDefault();
                if(projectRecord == null)
                {
                    return NotFound();
                }

                DateTime now = TimeProvider.CurrentTime;

                projectRecord.Title = project.Title;
                projectRecord.Descritpion = project.Descritpion;
                projectRecord.Color = project.Color;
                projectRecord.UpdateDate = now;

                DB.SaveChanges();

                var response = DecorateProjectItems(SelectProjects(projectRecord.PublicId), true).FirstOrDefault();

                return Ok(response);
            });
        }

        [Authorize]
        [HttpDelete]
        [Route("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<bool> RemoveProject(string id)
        {
            return HandleWebRequest<bool>(() =>
            {
                var projectRecord = SelectProjects(id).FirstOrDefault();
                if (projectRecord == null)
                {
                    return NotFound();
                }

                DB.Projects.Remove(projectRecord);
                DB.SaveChanges();

                return Ok(true);
            });
        }
    }
}
