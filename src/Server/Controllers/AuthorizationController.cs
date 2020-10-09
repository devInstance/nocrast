using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NoCrast.Server.Database;
using NoCrast.Server.Indentity;
using NoCrast.Server.Model;
using NoCrast.Shared.Model;
using NoCrast.Shared.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoCrast.Server.Controllers
{
    [Route("api/user/account")]
    [ApiController]
    public class AuthorizationController : UserBaseController
    {
        private readonly IApplicationSignManager signInManager;

        public AuthorizationController(ApplicationDbContext db, UserManager<ApplicationUser> um, IApplicationSignManager sm)
            : base(db, um)
        {
            signInManager = sm;
        }

        [Route("signin")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login(LoginParameters parameters)
        {
            ResetCurrentProfile();

            var user = await UserManager.FindByNameAsync(parameters.UserName);
            if (user == null)
            {
                return Unauthorized("User does not exist");
            }

            var singInResult = await signInManager.SignInAsync(user, parameters.Password, parameters.RememberMe);
            if (!singInResult.Succeeded)
            {
                return Unauthorized("Invalid password");
            }

            if (CurrentProfile == null)
            {
                DB.UserProfiles.Add(new UserProfile
                {
                    Id = Guid.NewGuid(),
                    PublicId = IdGenerator.New(),
                    Status = UserStatus.LIVE,

                    ApplicationUserId = user.Id,
                    Name = parameters.UserName,
                    Email = parameters.UserName,

                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now
                });
            }
            DB.SaveChanges();

            return Ok();
        }

        [Route("register")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register(RegisterParameters parameters)
        {
            var user = new ApplicationUser();
            user.UserName = parameters.UserName;
            var result = await UserManager.CreateAsync(user, parameters.Password);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors.FirstOrDefault()?.Description);
            }

            return await Login(new LoginParameters {
                                    UserName = parameters.UserName,
                                    Password = parameters.Password
                                });
        }

        [Route("signout")]
        [Authorize]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            ResetCurrentProfile();
            return Ok();
        }

        [Route("user-info")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public UserInfoItem GetUserInfo()
        {
            return new UserInfoItem
            {
                IsAuthenticated = User.Identity.IsAuthenticated,
                UserName = User.Identity.Name,
                ExposedClaims = User.Claims.ToDictionary(c => c.Type, c => c.Value)
            };
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Delete()
        {
            await signInManager.SignOutAsync();

            var user = await UserManager.FindByNameAsync(CurrentProfile.Email);
            if (user == null)
            {
                return Unauthorized("User does not exist");
            }

            db.TagToTimerTasks.RemoveRange(
                (
                from items in db.TagToTimerTasks 
                 join ts in db.Tasks on items.Task equals ts
                 where ts.Profile == CurrentProfile 
                 select items
                 ).ToList()
            );

            db.TimerTags.RemoveRange((from items in db.TimerTags 
                                      where items.Profile == CurrentProfile select items).ToList());

            db.TaskState.RemoveRange((from items in db.TaskState
                                      join ts in db.Tasks on items.Task equals ts
                                      where ts.Profile == CurrentProfile
                                      select items).ToList());

            db.TimeLog.RemoveRange((from items in db.TimeLog
                                    join ts in db.Tasks on items.Task equals ts
                                    where ts.Profile == CurrentProfile
                                    select items).ToList());

            db.Tasks.RemoveRange((from items in db.Tasks
                                  where items.Profile == CurrentProfile
                                  select items).ToList());

            db.Projects.RemoveRange((from items in db.Projects
                                     where items.Profile == CurrentProfile
                                     select items).ToList());

            DB.UserProfiles.Remove(CurrentProfile);

            DB.SaveChanges();

            var result = await UserManager.DeleteAsync(user);
            ResetCurrentProfile();

            return Ok(true);
        }

    }
}
