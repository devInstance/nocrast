using System;
using System.Collections.Generic;
using System.Text;

namespace NoCrast.Shared.Model
{
    public class UserInfo
    {
        public bool IsAuthenticated { get; set; }
        public string UserName { get; set; }
        public string PublicId { get; set; }
        public Dictionary<string, string> ExposedClaims { get; set; }
    }
}
