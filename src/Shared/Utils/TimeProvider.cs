using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoCrast.Shared.Utils
{
    public class TimeProvider : ITimeProvider
    {
        public DateTime CurrentTime
        {
            get
            {
                return DateTime.Now;
            }
        }
    }
}
