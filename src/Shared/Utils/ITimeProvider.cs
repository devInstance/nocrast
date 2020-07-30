using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoCrast.Shared.Utils
{
    public interface ITimeProvider
    {
        DateTime CurrentTime { get; }

        int UtcTimeOffset { get; }
    }
}
