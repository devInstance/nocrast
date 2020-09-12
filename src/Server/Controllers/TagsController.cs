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

    }
}
