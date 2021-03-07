using NoCrast.Server.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoCrast.Server.Data.Queries
{
    public interface ITasksQuery
    {
        List<TimerTask> SelectList();
    }
}
