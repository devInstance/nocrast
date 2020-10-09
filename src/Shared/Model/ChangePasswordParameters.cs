using System;
using System.Collections.Generic;
using System.Text;

namespace NoCrast.Shared.Model
{
    public class ChangePasswordParameters
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
