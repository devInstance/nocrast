using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoCrast.Server.Model
{
    public enum UserStatus
    {
        LIVE = 1,
        SUSPENDED = 2
    }

    public class UserProfile : DatabaseObject
    {
        public string Email { get; set; }
        public string Name { get; set; }

        public Guid ApplicationUserId { get; set; }

        public UserStatus Status { get; set; }
    }
}
