using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NoCrast.Server.Model;
using NoCrast.Shared.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoCrast.Server.Controllers
{
    [Route("api/user/account")]
    [ApiController]
    public class AuthorizationController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;

        public AuthorizationController(UserManager<ApplicationUser> um, SignInManager<ApplicationUser> sm)
        {
            userManager = um;
            signInManager = sm;
        }

        [Route("signin")]
        [HttpPost]
        public async Task<IActionResult> Login(LoginParameters parameters)
        {
            var user = await userManager.FindByNameAsync(parameters.UserName);
            if (user == null)
            {
                return Unauthorized("User does not exist");
            }

            var singInResult = await signInManager.CheckPasswordSignInAsync(user, parameters.Password, false);

            if (!singInResult.Succeeded)
            {
                return Unauthorized("Invalid password");
            }

            await signInManager.SignInAsync(user, parameters.RememberMe);

            return Ok();
        }


        [Route("register")]
        [HttpPost]
        public async Task<IActionResult> Register(RegisterParameters parameters)
        {
            var user = new ApplicationUser();
            user.UserName = parameters.UserName;
            var result = await userManager.CreateAsync(user, parameters.Password);
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
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return Ok();
        }

        [Route("user-info")]
        [HttpGet]
        public UserInfoItem GetUserInfo()
        {
            return new UserInfoItem
            {
                IsAuthenticated = User.Identity.IsAuthenticated,
                UserName = User.Identity.Name,
                ExposedClaims = User.Claims.ToDictionary(c => c.Type, c => c.Value)
            };
        }
    }
}
