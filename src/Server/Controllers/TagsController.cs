using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    [Route("api/data/tags")]
    public class TagsController : UserBaseController
    {
        public ITimeProvider TimeProvider { get; }

        public TagsController(ApplicationDbContext d,
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
        public ActionResult<TagItem[]> GetTags()
        {
            return HandleWebRequest((WebHandler<TagItem[]>)(() =>
            {
                List<TagItem> result = DecorateTagItems(SelectTags(null));
                return Ok(result.ToArray());
            }));
        }

        private List<TagItem> DecorateTagItems(IQueryable<TimerTag> q)
        {
            return (from tags in q
                    select new TagItem
                    {
                        Id = tags.PublicId,
                        Name = tags.Name
                    }).ToList();
        }

        private IQueryable<TimerTag> SelectTags(string id)
        {
            return (from tags in DB.TimerTags
                    where tags.Profile == CurrentProfile && (id == null || tags.PublicId == id)
                    orderby tags.Name ascending
                    select tags);
        }

        private IQueryable<TimerTag> SelectTagsByName(string name)
        {
            return (from tags in DB.TimerTags
                    where tags.Profile == CurrentProfile && tags.Name == name
                    orderby tags.Name ascending
                    select tags);
        }

        [Authorize]
        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<TagItem> GetTag(string id)
        {
            return HandleWebRequest((WebHandler<TagItem>)(() =>
            {
                List<TagItem> result = DecorateTagItems(SelectTags(id));
                return Ok(result.FirstOrDefault());
            }));
        }

        [Authorize]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<TagItem> AddTag(TagItem tag)
        {
            return HandleWebRequest<TagItem>(() =>
            {
                DateTime now = TimeProvider.CurrentTime;

                //check if the tag with such name already exists
                var existingRecord = SelectTagsByName(tag.Name).FirstOrDefault();
                if(existingRecord != null)
                {
                    return new TagItem
                    {
                        Id = existingRecord.PublicId,
                        Name = existingRecord.Name
                    };
                }

                var tagRecord = new TimerTag
                {
                    Id = Guid.NewGuid(),
                    PublicId = IdGenerator.New(),

                    Name = tag.Name,
                    Profile = CurrentProfile,

                    CreateDate = now,
                    UpdateDate = now,
                };
                DB.TimerTags.Add(tagRecord);

                DB.SaveChanges();

                var response = DecorateTagItems(SelectTags(tagRecord.PublicId)).FirstOrDefault();

                return Ok(response);
            });
        }

        [Authorize]
        [HttpPut]
        [Route("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<TagItem> UpdateTag(string id, [FromBody] TagItem tag)
        {
            return HandleWebRequest<TagItem>(() =>
            {
                var tagRecord = SelectTags(id).FirstOrDefault();
                if(tagRecord == null)
                {
                    return NotFound();
                }

                DateTime now = TimeProvider.CurrentTime;

                tagRecord.Name = tag.Name;
                tagRecord.UpdateDate = now;

                DB.SaveChanges();

                var response = DecorateTagItems(SelectTags(tagRecord.PublicId)).FirstOrDefault();

                return Ok(response);
            });
        }

        [Authorize]
        [HttpDelete]
        [Route("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<bool> RemoveTag(string id)
        {
            return HandleWebRequest<bool>(() =>
            {
                var tagRecord = SelectTags(id).FirstOrDefault();
                if (tagRecord == null)
                {
                    return NotFound();
                }

                DB.TimerTags.Remove(tagRecord);
                DB.SaveChanges();

                return Ok(true);
            });
        }

        [Authorize]
        [HttpGet]
        [Route("task/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<TagItem[]> GetTagsByTaskIdAsync(string id)
        {
            return HandleWebRequest<TagItem[]>(() =>
            {
                var tags = SelectTags(null);

                var dtags = (from tag in tags
                                join ttt in DB.TagToTimerTasks on tag equals ttt.Tag
                                join tsk in DB.Tasks on ttt.Task equals tsk
                                where tsk.PublicId == id && tsk.Profile == CurrentProfile
                                select tag);

                List<TagItem> result = DecorateTagItems(dtags);
                return Ok(result.ToArray());
            });
        }

        [Authorize]
        [HttpPost]
        [Route("{tagId}/task")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public ActionResult<TagItem> AddTagTaskAsync([FromBody]string taskId, string tagId)
        {
            return HandleWebRequest<TagItem>(() =>
            {
                var tag = SelectTags(tagId).FirstOrDefault();
                if(tag == null)
                {
                    return NotFound();
                }

                var taskDbId = (from task in DB.Tasks
                            where task.PublicId == taskId && task.Profile == CurrentProfile
                            select task.Id).FirstOrDefault();
                if(taskDbId == null)
                {
                    return NotFound();
                }

                var existing = (from ttt in DB.TagToTimerTasks
                                where ttt.TagId == tag.Id && ttt.TaskId == taskDbId
                                select ttt).FirstOrDefault();
                if(existing != null)
                {
                    return Conflict();
                }

                var tagRecord = new TagToTimerTask
                {
                    Id = Guid.NewGuid(),
                    TagId = tag.Id,
                    TaskId = taskDbId
                };

                DB.TagToTimerTasks.Add(tagRecord);

                DB.SaveChanges();

                var response = DecorateTagItems(SelectTags(tagId)).FirstOrDefault();

                return Ok(response);
            });
        }

        [Authorize]
        [HttpDelete]
        [Route("{tagId}/task/{taskId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<bool> RemoveTagTaskAsync(string taskId, string tagId)
        {
            return HandleWebRequest<bool>(() =>
            {
                var record = (from ttt in DB.TagToTimerTasks
                              join task in DB.Tasks on ttt.Task equals task
                              join tag in DB.TimerTags on ttt.Tag equals tag
                              where task.PublicId == taskId && task.Profile == CurrentProfile
                                    && tag.PublicId == tagId && tag.Profile == CurrentProfile
                              select ttt).FirstOrDefault();
                if (record == null)
                {
                    return NotFound();
                }

                DB.TagToTimerTasks.Remove(record);
                DB.SaveChanges();

                return Ok(true);
            });
        }

    }
}
