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
using System.Threading.Tasks;

namespace NoCrast.Server.Controllers
{
    [Route("api/user/profile")]
    [ApiController]
    public class UserProfileController : UserBaseController
    {
        public ITimeProvider TimeProvider { get; }

        public UserProfileController(ApplicationDbContext d,
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
        public ActionResult<UserProfileItem> GetProfile()
        {
            return HandleWebRequest((WebHandler<UserProfileItem>)(() =>
            {
                return Ok(new UserProfileItem
                {
                    Name = CurrentProfile.Name,
                    Email = CurrentProfile.Email
                }
                );
            }));
        }

        [Authorize]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<UserProfileItem> UpdateProfile([FromBody] UserProfileItem newProfile)
        {
            return HandleWebRequest((WebHandler<UserProfileItem>)(() =>
            {
                var profile = CurrentProfile;
                profile.Name = newProfile.Name;
                profile.UpdateDate = TimeProvider.CurrentTime;
                DB.SaveChanges();

                return Ok(new UserProfileItem
                {
                    Name = profile.Name,
                    Email = profile.Email
                }
                );
            }));
        }

    }
}
