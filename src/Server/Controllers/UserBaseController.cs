using Microsoft.AspNetCore.Identity;
using NoCrast.Server.Database;
using NoCrast.Server.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoCrast.Server.Controllers
{
    public class UserBaseController : BaseController
    {
        protected UserManager<ApplicationUser> UserManager { get; }

        protected UserBaseController(ApplicationDbContext d, UserManager<ApplicationUser> userManager) : base(d)
        {
            this.UserManager = userManager;
        }

        private UserProfile currentProfile;
        public UserProfile CurrentProfile
        {
            get
            {
                if (currentProfile == null)
                {
                    string userId = UserManager.GetUserId(this.User);
                    if (!String.IsNullOrEmpty(userId))
                    {
                        currentProfile = (from pr in db.UserProfiles
                                          where pr.ApplicationUserId != null && pr.ApplicationUserId == Guid.Parse(userId)
                                          select pr).FirstOrDefault();
                    }
                }
                return currentProfile;
            }
        }

        public void ResetCurrentProfile()
        {
            currentProfile = null;
        }
    }
}